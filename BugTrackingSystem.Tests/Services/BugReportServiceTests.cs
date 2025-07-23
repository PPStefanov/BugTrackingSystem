using BugTrackingSystem.Data;
using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.Services.Core;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BugTrackingSystem.Tests.Services
{
    [TestFixture]
    public class BugReportServiceTests
    {
        private BugTrackingSystemDbContext _context;
        private BugReportService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BugTrackingSystemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BugTrackingSystemDbContext(options);
            _service = new BugReportService(_context, null); // UserManager can be null for testing

            // Seed test data
            SeedTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SeedTestData()
        {
            var priority = new BugPriorityEntity { Id = 1, Name = "High" };
            var status = new BugStatusEntity { Id = 1, Name = "Open" };
            var application = new ApplicationName { Id = 1, Name = "Test App", Description = "Test Application" };

            _context.BugPriorities.Add(priority);
            _context.BugStatuses.Add(status);
            _context.ApplicationName.Add(application);
            _context.SaveChanges();
        }

        [Test]
        public async Task GetAllBugReportsAsync_ShouldReturnAllBugReports()
        {
            // Arrange
            var bugReport = new BugReport
            {
                Id = 1,
                Title = "Test Bug",
                Description = "Test Description",
                PriorityId = 1,
                StatusId = 1,
                ApplicationId = 1,
                ReporterId = "test-user-id",
                AssignedToUserId = "test-assignee-id",
                CreatedAt = DateTime.Now
            };

            _context.BugReports.Add(bugReport);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllBugsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Test Bug"));
        }

        [Test]
        public async Task GetBugReportByIdAsync_WithValidId_ShouldReturnBugReport()
        {
            // Arrange
            var bugReport = new BugReport
            {
                Id = 1,
                Title = "Test Bug",
                Description = "Test Description",
                PriorityId = 1,
                StatusId = 1,
                ApplicationId = 1,
                ReporterId = "test-user-id",
                AssignedToUserId = "test-assignee-id",
                CreatedAt = DateTime.Now
            };

            _context.BugReports.Add(bugReport);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Test Bug"));
        }

        [Test]
        public async Task GetBugReportByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateBugReportAsync_WithValidData_ShouldCreateBugReport()
        {
            // Arrange
            var bugReport = new BugReport
            {
                Title = "New Bug",
                Description = "New Description",
                PriorityId = 1,
                StatusId = 1,
                ApplicationId = 1,
                ReporterId = "test-user-id",
                AssignedToUserId = "test-assignee-id",
                CreatedAt = DateTime.Now
            };

            // Act
            var result = await _service.CreateAsync(bugReport, null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Title, Is.EqualTo("New Bug"));

            var savedBug = await _context.BugReports.FindAsync(result.Id);
            Assert.That(savedBug, Is.Not.Null);
        }

        [Test]
        public async Task UpdateBugReportAsync_WithValidData_ShouldUpdateBugReport()
        {
            // Arrange
            var bugReport = new BugReport
            {
                Id = 1,
                Title = "Original Title",
                Description = "Original Description",
                PriorityId = 1,
                StatusId = 1,
                ApplicationId = 1,
                ReporterId = "test-user-id",
                AssignedToUserId = "test-assignee-id",
                CreatedAt = DateTime.Now
            };

            _context.BugReports.Add(bugReport);
            await _context.SaveChangesAsync();

            // Act
            bugReport.Title = "Updated Title";
            var result = await _service.UpdateAsync(bugReport, null);

            // Assert
            Assert.That(result, Is.True);

            var updatedBug = await _context.BugReports.FindAsync(1);
            Assert.That(updatedBug.Title, Is.EqualTo("Updated Title"));
        }

        [Test]
        public async Task DeleteBugReportAsync_WithValidId_ShouldDeleteBugReport()
        {
            // Arrange
            var bugReport = new BugReport
            {
                Id = 1,
                Title = "Test Bug",
                Description = "Test Description",
                PriorityId = 1,
                StatusId = 1,
                ApplicationId = 1,
                ReporterId = "test-user-id",
                AssignedToUserId = "test-assignee-id",
                CreatedAt = DateTime.Now
            };

            _context.BugReports.Add(bugReport);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteAsync(1, null);

            // Assert
            Assert.That(result, Is.True);

            var deletedBug = await _context.BugReports.FindAsync(1);
            Assert.That(deletedBug, Is.Null);
        }

        [Test]
        public async Task DeleteBugReportAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Act
            var result = await _service.DeleteAsync(999, null);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}