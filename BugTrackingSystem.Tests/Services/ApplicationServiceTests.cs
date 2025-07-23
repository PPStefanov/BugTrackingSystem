using BugTrackingSystem.Data;
using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.Services.Core;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BugTrackingSystem.Tests.Services
{
    [TestFixture]
    public class ApplicationServiceTests
    {
        private BugTrackingSystemDbContext _context;
        private ApplicationService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BugTrackingSystemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BugTrackingSystemDbContext(options);
            _service = new ApplicationService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAllApplicationsAsync_ShouldReturnAllApplications()
        {
            // Arrange
            var app1 = new ApplicationName { Id = 1, Name = "App1", Description = "First App", IsActive = true };
            var app2 = new ApplicationName { Id = 2, Name = "App2", Description = "Second App", IsActive = true };

            _context.ApplicationName.AddRange(app1, app2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllApplicationsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetApplicationByIdAsync_WithValidId_ShouldReturnApplication()
        {
            // Arrange
            var app = new ApplicationName { Id = 1, Name = "Test App", Description = "Test Description", IsActive = true };
            _context.ApplicationName.Add(app);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetApplicationByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Test App"));
        }

        [Test]
        public async Task GetApplicationByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetApplicationByIdAsync(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateApplicationAsync_WithValidData_ShouldCreateApplication()
        {
            // Arrange
            var app = new ApplicationName { Name = "New App", Description = "New Description" };

            // Act
            var result = await _service.CreateApplicationAsync(app);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Name, Is.EqualTo("New App"));
            Assert.That(result.IsActive, Is.True);
            Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public async Task UpdateApplicationAsync_WithValidData_ShouldUpdateApplication()
        {
            // Arrange
            var app = new ApplicationName { Id = 1, Name = "Original Name", Description = "Original Description", IsActive = true };
            _context.ApplicationName.Add(app);
            await _context.SaveChangesAsync();

            // Act
            app.Name = "Updated Name";
            app.Description = "Updated Description";
            var result = await _service.UpdateApplicationAsync(app);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Updated Name"));
            Assert.That(result.Description, Is.EqualTo("Updated Description"));
            Assert.That(result.UpdatedAt, Is.Not.Null);
        }

        [Test]
        public async Task ToggleApplicationStatusAsync_ShouldToggleIsActiveStatus()
        {
            // Arrange
            var app = new ApplicationName { Id = 1, Name = "Test App", Description = "Test Description", IsActive = true };
            _context.ApplicationName.Add(app);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ToggleApplicationStatusAsync(1);

            // Assert
            Assert.That(result, Is.True);
            
            var updatedApp = await _context.ApplicationName.FindAsync(1);
            Assert.That(updatedApp.IsActive, Is.False);
            Assert.That(updatedApp.UpdatedAt, Is.Not.Null);
        }

        [Test]
        public async Task ToggleApplicationStatusAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Act
            var result = await _service.ToggleApplicationStatusAsync(999);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteApplicationAsync_WithValidId_ShouldDeleteApplication()
        {
            // Arrange
            var app = new ApplicationName { Id = 1, Name = "Test App", Description = "Test Description", IsActive = true };
            _context.ApplicationName.Add(app);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteApplicationAsync(1);

            // Assert
            Assert.That(result, Is.True);

            var deletedApp = await _context.ApplicationName.FindAsync(1);
            Assert.That(deletedApp, Is.Null);
        }

        [Test]
        public async Task DeleteApplicationAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Act
            var result = await _service.DeleteApplicationAsync(999);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}