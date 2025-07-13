using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.GCommon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTrackingSystem.Data.Configurations
{
    public class BugReportConfiguration : IEntityTypeConfiguration<BugReport>
    {
        public void Configure(EntityTypeBuilder<BugReport> entity)
        {
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(ValidationConstants.BugReport.TitleMaxLength);

            entity.Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(ValidationConstants.BugReport.DescriptionMaxLength);

            entity.HasOne(b => b.Reporter)
                .WithMany(u => u.ReportedBugs)
                .HasForeignKey(b => b.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.ApplicationName)
                .WithMany(a => a.BugReports)
                .HasForeignKey(b => b.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.AssignedToUser)
                .WithMany()
                .HasForeignKey(b => b.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(b => b.IsActive); // Soft-delete filter
        }
    }
}
