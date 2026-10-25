using InterviewsOrganizer.Shared;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection in appsettings.json.");

var keyRingPath = configuration["EmailEncryption:KeyRingPath"] ?? "../keys";
var keyRingDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, keyRingPath));

// Must use the exact same application name and key ring directory as the API so that the
// generated protector produces/reads identical ciphertext, and the same HMAC secret so
// ComputeEmailHash matches values stored by the API.
var dataProtectionProvider = DataProtectionProvider.Create(
    keyRingDirectory,
    builder => builder.SetApplicationName("InterviewsOrganizer"));

IEmailEncryptionService emailEncryptionService = new EmailEncryptionService(dataProtectionProvider, configuration);

string? email = args.Length > 0 ? args[0] : null;
if (string.IsNullOrWhiteSpace(email))
{
    Console.Write("Email: ");
    email = Console.ReadLine();
}

if (string.IsNullOrWhiteSpace(email))
{
    Console.Error.WriteLine("No email provided.");
    return 1;
}

var emailHash = emailEncryptionService.ComputeEmailHash(email);

Console.WriteLine($"Input Email: {email}");
Console.WriteLine($"EmailHash:   {emailHash}");

await using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();

const string sql = "SELECT Id FROM AspNetUsers WHERE EmailHash = @EmailHash";
await using var command = new SqlCommand(sql, connection);
command.Parameters.AddWithValue("@EmailHash", emailHash);

await using var reader = await command.ExecuteReaderAsync();
if (await reader.ReadAsync())
{
    Console.WriteLine($"UserId:      {reader.GetString(0)}");
}
else
{
    Console.WriteLine("UserId:      (no matching user found)");
}

Console.ReadKey();

return 0;

