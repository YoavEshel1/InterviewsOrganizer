using InterviewsOrganizer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewsOrganizer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Interview> Interviews { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
