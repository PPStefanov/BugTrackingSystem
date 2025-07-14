using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;


public interface ICommentService
{
    Task<List<Comment>> GetCommentsForBugAsync(int bugReportId);
    Task<Comment> GetByIdAsync(int id);
    Task<Comment> CreateAsync(Comment comment);
    Task<bool> UpdateAsync(Comment comment);
    Task<bool> DeleteAsync(int id);
}