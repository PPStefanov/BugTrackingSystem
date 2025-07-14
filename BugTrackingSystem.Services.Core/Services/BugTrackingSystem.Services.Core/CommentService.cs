using BugTrackingSystem.Data;
using BugTrackingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CommentService : ICommentService
{
    private readonly BugTrackingSystemDbContext _dbContext;

    public CommentService(BugTrackingSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Comment>> GetCommentsForBugAsync(int bugReportId)
    {
        return await _dbContext.Comments
            .Include(c => c.Author)
            .Where(c => c.BugReportId == bugReportId && c.IsActive)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comment> GetByIdAsync(int id)
    {
        return await _dbContext.Comments
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
    }

    public async Task<Comment> CreateAsync(Comment comment)
    {
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> UpdateAsync(Comment comment)
    {
        var existing = await _dbContext.Comments.FindAsync(comment.Id);
        if (existing == null || !existing.IsActive)
            return false;

        existing.Content = comment.Content;
        // Optionally update other fields
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var comment = await _dbContext.Comments.FindAsync(id);
        if (comment == null || !comment.IsActive)
            return false;

        comment.IsActive = false;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}