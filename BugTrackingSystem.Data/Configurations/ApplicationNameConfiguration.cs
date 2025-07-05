using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugTrackingSystem.Models.Entities;

namespace BugTrackingSystem.Data.Configuration
{
    public class ApplicationNameConfiguration : IEntityTypeConfiguration<ApplicationName>
    {
        public void Configure(EntityTypeBuilder<ApplicationName> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(a => a.BugReports)
                .WithOne(b => b.ApplicationName)
                .HasForeignKey(b => b.Id);
        }
    }
}
