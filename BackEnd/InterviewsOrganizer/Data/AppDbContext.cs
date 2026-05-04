using InterviewsOrganizer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewsOrganizer.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Interview> Interviews => Set<Interview>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Position> Positions => Set<Position>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Position>()
                .HasOne<Company>()
                .WithMany(c => c.Positions)
                .HasForeignKey(p => p.CompanyId);
        }
    }
}
