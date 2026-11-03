using InterviewsOrganizer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewsOrganizer.Data.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("UserProfiles");

            builder.HasKey(p => p.UserId);

            builder.Property(p => p.UserId)
                .HasColumnType("nvarchar(450)")
                .IsRequired();

            builder.Property(p => p.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .HasColumnType("datetimeoffset");

            builder.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
