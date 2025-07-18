using BugTrackingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTrackingSystem.Data.Configurations
{
    public class AdminSubscriptionConfiguration : IEntityTypeConfiguration<AdminSubscription>
    {
        public void Configure(EntityTypeBuilder<AdminSubscription> builder)
        {
            // Configure Admin relationship
            builder.HasOne(s => s.Admin)
                .WithMany(u => u.AdminSubscriptions)
                .HasForeignKey(s => s.AdminId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure BugReport relationship
            builder.HasOne(s => s.BugReport)
                .WithMany()
                .HasForeignKey(s => s.BugReportId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure unique subscription per admin per ticket
            builder.HasIndex(s => new { s.AdminId, s.BugReportId })
                .IsUnique();
        }
    }
}