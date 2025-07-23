using BugTrackingSystem.Services.Core;
using BugTrackingSystem.ViewModels.Dashboard;
using BugTrackingSystem.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BugTrackingSystem.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<ILogger<HomeController>> _mockLogger;
        private Mock<IDashboardService> _mockDashboardService;
        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockDashboardService = new Mock<IDashboardService>();
            _controller = new HomeController(_mockLogger.Object, _mockDashboardService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task Index_ShouldReturnViewWithDashboardViewModel()
        {
            // Arrange
            var mockStats = new DashboardStatsViewModel
            {
                TotalBugReports = 10,
                ResolvedIssues = 5,
                PendingReview = 5,
                ActiveUsers = 3
            };

            var mockActivity = new List<RecentActivityViewModel>
            {
                new RecentActivityViewModel
                {
                    Title = "Test Bug",
                    Action = "Created",
                    UserName = "TestUser",
                    Timestamp = DateTime.Now,
                    Status = "Open",
                    Priority = "High",
                    ActivityType = "Bug Report"
                }
            };

            var mockBugsByStatus = new Dictionary<string, int>
            {
                { "Open", 5 }
            };

            var mockBugsByPriority = new Dictionary<string, int>
            {
                { "High", 3 }
            };

            var mockMonthlyStats = new List<MonthlyStatsViewModel>
            {
                new MonthlyStatsViewModel { Month = "January", Created = 5, Resolved = 3 }
            };

            _mockDashboardService.Setup(x => x.GetDashboardStatsAsync()).ReturnsAsync(mockStats);
            _mockDashboardService.Setup(x => x.GetRecentActivityAsync(10)).ReturnsAsync(mockActivity);
            _mockDashboardService.Setup(x => x.GetBugsByStatusAsync()).ReturnsAsync(mockBugsByStatus);
            _mockDashboardService.Setup(x => x.GetBugsByPriorityAsync()).ReturnsAsync(mockBugsByPriority);
            _mockDashboardService.Setup(x => x.GetMonthlyStatsAsync(It.IsAny<int>())).ReturnsAsync(mockMonthlyStats);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.InstanceOf<DashboardViewModel>());
            
            var model = viewResult.Model as DashboardViewModel;
            Assert.That(model.Stats.TotalBugReports, Is.EqualTo(10));
            Assert.That(model.RecentActivity.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Privacy_ShouldReturnView()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task Index_WhenServiceThrowsException_ShouldHandleGracefully()
        {
            // Arrange
            _mockDashboardService.Setup(x => x.GetDashboardStatsAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _controller.Index());
        }

        [Test]
        public async Task Index_WithNullStats_ShouldHandleGracefully()
        {
            // Arrange
            _mockDashboardService.Setup(x => x.GetDashboardStatsAsync()).ReturnsAsync((DashboardStatsViewModel)null);
            _mockDashboardService.Setup(x => x.GetRecentActivityAsync(It.IsAny<int>())).ReturnsAsync(new List<RecentActivityViewModel>());
            _mockDashboardService.Setup(x => x.GetBugsByStatusAsync()).ReturnsAsync(new Dictionary<string, int>());
            _mockDashboardService.Setup(x => x.GetBugsByPriorityAsync()).ReturnsAsync(new Dictionary<string, int>());
            _mockDashboardService.Setup(x => x.GetMonthlyStatsAsync(It.IsAny<int>())).ReturnsAsync(new List<MonthlyStatsViewModel>());

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }
    }
}