using System.Collections.Generic;
using System.Threading.Tasks;
using BugTrackingSystem.Models.Entities;

namespace BugTrackingSystem.Services.Interfaces
{
    public interface IBugService
    {
        Task<Bug> GetBugByIdAsync(int id);
        Task<IEnumerable<Bug>> GetAllBugsAsync();
        Task<Bug> CreateBugAsync(Bug bug);
        Task<Bug> UpdateBugAsync(Bug bug);
        Task<bool> DeleteBugAsync(int id);
    }
}