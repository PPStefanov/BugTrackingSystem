using BugTrackingSystem.Data;
using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.Services.Core;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BugTrackingSystem.Tests.Services
{
    [TestFixture]
    public class DashboardServiceTests
    {
        private BugTrackingSystemDbContext _context;
        private DashboardService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BugTrackingSystemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BugTrackingSystemDbContext(options);
            _service = new DashboardService(_context);

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
            var status1 = new BugStatusEntity { Id = 1, Name = "Open" };
            var status2 = new BugStatusEntity { Id = 2, Name = "Closed" };
            var application = new ApplicationName { Id = 1, Name = "Test App", Description = "Test Application" };

            _context.BugPriorities.Add(priority);
            _context.BugStatuses.AddRange(status1, status2);
            _context.ApplicationName.Add(application);

            // Add some bug reports
            var bugs = new List<BugReport>
            {
                new BugReport
                {
                    Id = 1,
                    Title = "Bug 1",
                    Description = "Description 1",
                    PriorityId = 1,
                    StatusId = 1,
                    ApplicationId = 1,
                    ReporterId = "user1",
                    AssignedToUserId = "user2",
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new BugReport
                {
                    Id = 2,
                    Title = "Bug 2",
                    Description = "Description 2",
                    PriorityId = 1,
                    StatusId = 2,
                    ApplicationId = 1,
                    ReporterId = "user1",
                    AssignedToUserId = "user2",
                    CreatedAt = DateTime.Now.AddDays(-2)
                }
            };

            _context.BugReports.AddRange(bugs);
            _context.SaveChanges();
        }

        [Test]
        public async Task GetDashboardStatsAsync_ShouldReturnCorrectStats()
        {
            // Act
            var result = await _service.GetDashboardStatsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalBugReports, Is.EqualTo(2));
            Assert.That(result.ResolvedIssues, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.PendingReview, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.ActiveUsers, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task GetRecentActivityAsync_ShouldReturnRecentBugs()
        {
            // Act
            var result = await _service.GetRecentActivityAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));
            
            var firstActivity = result.First();
            Assert.That(firstActivity.Title, Is.Not.Null.And.Not.Empty);
            Assert.That(firstActivity.Action, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task GetBugsByStatusAsync_ShouldReturnStatusDistribution()
        {
            // Act
            var result = await _service.GetBugsByStatusAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));
            
            var totalCount = result.Sum(x => x.Value);
            Assert.That(totalCount, Is.EqualTo(2));
        }

        [Test]
        public async Task GetBugsByPriorityAsync_ShouldReturnPriorityDistribution()
        {
            // Act
            var result = await _service.GetBugsByPriorityAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));
            
            var totalCount = result.Sum(x => x.Value);
            Assert.That(totalCount, Is.EqualTo(2));
        }

        [Test]
        public async Task GetMonthlyStatsAsync_ShouldReturnMonthlyData()
        {
            // Act
            var result = await _service.GetMonthlyStatsAsync(6);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.LessThanOrEqualTo(6));
            
            if (result.Any())
            {
                var firstMonth = result.First();
                Assert.That(firstMonth.Month, Is.Not.Null.And.Not.Empty);
                Assert.That(firstMonth.Created, Is.GreaterThanOrEqualTo(0));
                Assert.That(firstMonth.Resolved, Is.GreaterThanOrEqualTo(0));
            }
        }

        [Test]
        public void GetDashboardStatsAsync_WithEmptyDatabase_ShouldReturnZeroStats()
        {
            // Arrange - Clear all data
            _context.BugReports.RemoveRange(_context.BugReports);
            _context.SaveChanges();

            // Act & Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                var result = await _service.GetDashboardStatsAsync();
                Assert.That(result.TotalBugReports, Is.EqualTo(0));
                Assert.That(result.ResolvedIssues, Is.EqualTo(0));
                Assert.That(result.PendingReview, Is.EqualTo(0));
            });
        }
    }
}