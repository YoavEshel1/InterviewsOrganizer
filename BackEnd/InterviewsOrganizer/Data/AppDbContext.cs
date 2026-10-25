using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using InterviewsOrganizer.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InterviewsOrganizer.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService, IEmailEncryptionService emailEncryptionService) : IdentityDbContext<AppUser>(options)
    {
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly IEmailEncryptionService _emailEncryptionService = emailEncryptionService;

        public DbSet<Interview> Interviews => Set<Interview>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<AppUser>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    var email = entry.Entity.Email;
                    entry.Entity.EmailHash = string.IsNullOrEmpty(email)
                        ? string.Empty
                        : _emailEncryptionService.ComputeEmailHash(email);
                    entry.Entity.EmailEncryptedAt = DateTime.UtcNow;
                }
            }

            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                    entry.Property(x => x.CreatedAt).IsModified = false;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var emailConverter = new ValueConverter<string?, string?>(
                plain => plain == null ? null : _emailEncryptionService.Encrypt(plain),
                cipher => cipher == null ? null : _emailEncryptionService.Decrypt(cipher));

            modelBuilder.Entity<AppUser>()
                .Property(u => u.Email)
                .HasConversion(emailConverter);

            modelBuilder.Entity<AppUser>()
                .Property(u => u.NormalizedEmail)
                .HasConversion(emailConverter);

            // Unique: safe to enforce now that EmailEncryptionMigrator has backfilled EmailHash for
            // every pre-existing row (this index is added by a migration that runs strictly after
            // the backfill — see Program.cs startup ordering).
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.EmailHash)
                .IsUnique();

            modelBuilder.Entity<Position>()
                .HasOne<Company>()
                .WithMany(c => c.Positions)
                .HasForeignKey(p => p.CompanyId);

            modelBuilder.Entity<Company>()
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Position>()
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Interview>()
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Global query filters: scope tenant data to the current user.
            // UserId is null outside an HTTP request (e.g. migrations, background jobs);
            // in that case the filter is bypassed so cleanup/admin flows using
            // IgnoreQueryFilters() still function correctly.
            modelBuilder.Entity<Company>()
                .HasQueryFilter(c => _currentUserService.UserId == null || c.UserId == _currentUserService.UserId);

            modelBuilder.Entity<Position>()
                .HasQueryFilter(p => _currentUserService.UserId == null || p.UserId == _currentUserService.UserId);

            modelBuilder.Entity<Interview>()
                .HasQueryFilter(i => _currentUserService.UserId == null || i.UserId == _currentUserService.UserId);
        }
    }
}
