using BugTrackingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTrackingSystem.Data.Configurations
{
    public class BugReportConfiguration : IEntityTypeConfiguration<BugReport>
    {
        public void Configure(EntityTypeBuilder<BugReport> builder)
        {
            // Configure Reporter relationship
            builder.HasOne(b => b.Reporter)
                .WithMany(u => u.ReportedBugs)
                .HasForeignKey(b => b.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure AssignedToUser relationship (QA)
            builder.HasOne(b => b.AssignedToUser)
                .WithMany(u => u.AssignedBugs)
                .HasForeignKey(b => b.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Developer relationship
            builder.HasOne(b => b.Developer)
                .WithMany(u => u.DeveloperBugs)
                .HasForeignKey(b => b.DeveloperId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Status relationship
            builder.HasOne(b => b.Status)
                .WithMany()
                .HasForeignKey(b => b.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Priority relationship
            builder.HasOne(b => b.Priority)
                .WithMany()
                .HasForeignKey(b => b.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Application relationship
            builder.HasOne(b => b.ApplicationName)
                .WithMany(a => a.BugReports)
                .HasForeignKey(b => b.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}