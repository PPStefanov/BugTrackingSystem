using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugTrackingSystem.Models.Entities;

namespace BugTrackingSystem.Data.Configuration
{
    public class BugPriorityConfiguration : IEntityTypeConfiguration<BugPriorityEntity>
    {
        public void Configure(EntityTypeBuilder<BugPriorityEntity> builder)
        {
            builder.ToTable("BugPriorities");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
