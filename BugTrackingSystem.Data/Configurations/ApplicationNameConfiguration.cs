using BugTrackingSystem.GCommon;
using BugTrackingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTrackingSystem.Data.Configuration
{
    public class ApplicationNameConfiguration : IEntityTypeConfiguration<ApplicationName>
    {
        public void Configure(EntityTypeBuilder<ApplicationName> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired();
                //.HasMaxLength(ValidationConstants.ApplicationNameValidation.NameMinLength);

            builder.HasMany(a => a.BugReports)
                .WithOne(b => b.ApplicationName)
                .HasForeignKey(b => b.Id);
        }
    }
}
