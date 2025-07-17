using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Data;

namespace BugTrackingSystem.Web.Controllers.Nomenclature
{
    public class ApplicationController : Controller
    {
        private readonly BugTrackingSystemDbContext _dbContext;

        public ApplicationController(BugTrackingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> List()
        {
            var apps = await _dbContext.ApplicationName
                .Select(a => new ApplicationViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsActive = a.IsActive
                }).ToListAsync();
            return View(apps);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = new ApplicationName
            {
                Name = model.Name,
                IsActive = model.IsActive
            };
            _dbContext.ApplicationName.Add(entity);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var app = await _dbContext.ApplicationName.FindAsync(id);
            if (app == null) return NotFound();

            var model = new ApplicationViewModel
            {
                Id = app.Id,
                Name = app.Name,
                IsActive = app.IsActive
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var app = await _dbContext.ApplicationName.FindAsync(model.Id);
            if (app == null) return NotFound();

            app.Name = model.Name;
            app.IsActive = model.IsActive;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var app = await _dbContext.ApplicationName.FindAsync(id);
            if (app == null) return NotFound();

            _dbContext.ApplicationName.Remove(app);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }
    }
}