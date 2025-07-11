using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Data;
using BugTrackingSystem.Models;
using BugTrackingSystem.Services.Interfaces;
using BugTrackingSystem.Models.Entities;



namespace BugTrackingSystem.Services
{
    public class BugService : IBugService
    {
        private readonly BugTrackingSystemDbContext _dBcontext;

        public BugService(BugTrackingSystemDbContext dBcontext)
        {
            _dBcontext = dBcontext;
        }

        public async Task<Bug> GetBugByIdAsync(int id)
        {
            return await _dBcontext.Bugs.FindAsync(id);
        }

        public async Task<IEnumerable<Bug>> GetAllBugsAsync()
        {
            return await _dBcontext.Bugs.ToListAsync();
        }

        public async Task<Bug> CreateBugAsync(Bug bug)
        {
            _dBcontext.Bugs.Add(bug);
            await _dBcontext.SaveChangesAsync();
            return bug;
        }

        public async Task<Bug> UpdateBugAsync(Bug bug)
        {
            _dBcontext.Bugs.Update(bug);
            await _dBcontext.SaveChangesAsync();
            return bug;
        }

        public async Task<bool> DeleteBugAsync(int id)
        {
            var bug = await _dBcontext.Bugs.FindAsync(id);
            if (bug == null)
                return false;

            _dBcontext.Bugs.Remove(bug);
            await _dBcontext.SaveChangesAsync();
            return true;
        }
    }
}