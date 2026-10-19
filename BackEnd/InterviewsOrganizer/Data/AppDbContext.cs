using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InterviewsOrganizer.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService) : IdentityDbContext<AppUser>(options)
    {
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public DbSet<Interview> Interviews => Set<Interview>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
