using BugTrackingSystem.GCommon;
using BugTrackingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTrackingSystem.Data.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.UserName)
                .HasMaxLength(ValidationConstants.AppUserValidation.AppUserMaxLength);

            builder.HasMany(u => u.ReportedBugs)
                .WithOne(b => b.Reporter)
                .HasForeignKey(b => b.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
