using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.GCommon;


namespace BugTrackingSystem.Data.Configuration
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Content)
                .HasMaxLength(ValidationConstants.Comment.ContentMaxLength)
                .IsRequired();

            builder.HasOne(c => c.BugReport)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BugReportId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(c => c.IsActive); // Soft-delete filter
        }
    }
}
