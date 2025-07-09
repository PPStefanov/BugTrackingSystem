using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.Models.Enums;

namespace BugTrackingSystem.Data.Configuration
{
    public class BugStatusConfiguration : IEntityTypeConfiguration<BugStatusEntity>
    {
        public void Configure(EntityTypeBuilder<BugStatusEntity> builder)
        {
            builder.ToTable("BugStatuses");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

        }
    }
}
