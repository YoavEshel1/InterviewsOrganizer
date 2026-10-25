using InterviewsOrganizer.Data;
using InterviewsOrganizer.Middleware;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Repositories;
using InterviewsOrganizer.Repositories.Interfaces;
using InterviewsOrganizer.Services;
using InterviewsOrganizer.Services.Interfaces;
using InterviewsOrganizer.Swagger;
using InterviewsOrganizer.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

// Bootstrap logger: captures anything that happens before the host (and its configured
// Serilog pipeline) is fully built, including host-startup failures.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting InterviewsOrganizer API");

    var builder = WebApplication.CreateBuilder(args);

    // Replace the default logging provider with Serilog, configured from appsettings.json ("Serilog" section).
    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Data Protection key ring is persisted to a shared filesystem directory so that both the API and the
// EmailHasher CLI (and any future instances) produce/decrypt identical ciphertext for AppUser emails.
var keyRingPath = builder.Configuration["EmailEncryption:KeyRingPath"] ?? "../keys";
builder.Services.AddDataProtection()
    .SetApplicationName("InterviewsOrganizer")
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, keyRingPath)));

builder.Services.AddSingleton<IEmailEncryptionService, EmailEncryptionService>();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Email is encrypted at rest; Identity's built-in unique-email enforcement operates on plaintext
    // and cannot be used. Uniqueness is instead enforced via the EmailHash blind index.
    options.User.RequireUniqueEmail = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"JWT AUTH FAILED: {context.Exception.GetType().Name}: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("JWT TOKEN VALIDATED OK");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"JWT CHALLENGE: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddRequestTimeouts(); // ← required for [RequestTimeout] attribute

builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        if (builder.Environment.IsDevelopment())
        {
            var exceptionFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature?.Error is { } ex)
            {
                context.ProblemDetails.Detail = ex.ToString();
            }
        }
    };
});

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();
builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IUserCleanupService, UserCleanupService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "InterviewsOrganizer", Version = "v1" });

    // Defines the "Bearer" security scheme used by the [Authorize]-driven operation filter below.
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter a valid JWT token. The \"Bearer \" prefix is added automatically."
    });

    // Adds the security requirement only to operations that require authorization.
    options.OperationFilter<AuthOperationFilter>();
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Seed super admin on startup.
// Migration ordering is critical for the encrypted-email backfill to be safe:
//   1. Apply the "AddEmailEncryptionColumns" migration (adds EmailHash/EmailEncryptedAt, non-unique index only).
//   2. Run EmailEncryptionMigrator to encrypt legacy plaintext emails, compute EmailHash, and scrub email-like UserNames.
//   3. Apply the remaining migrations (including the one that adds the unique index on EmailHash),
//      now that every row has a non-empty, populated hash.
//   4. Seed roles/super-admin using EmailHash lookups.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var migrator = dbContext.GetInfrastructure().GetRequiredService<Microsoft.EntityFrameworkCore.Migrations.IMigrator>();

    await migrator.MigrateAsync("AddEmailEncryptionColumns");
    await EmailEncryptionMigrator.RunAsync(scope.ServiceProvider);
    await dbContext.Database.MigrateAsync();

    await DbSeeder.SeedSuperAdminAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Interview Organizer API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseExceptionHandler(); // must be early in the pipeline so it can catch exceptions from later middleware/endpoints

// Logs one summary line per HTTP request (path, method, status code, elapsed time).
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseRequestTimeouts(); // ← required for [RequestTimeout] attribute
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    // HostAbortedException is thrown by `dotnet ef` design-time tooling; it is not a real failure.
    Log.Fatal(ex, "InterviewsOrganizer API terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
