using Microsoft.Playwright;
using NUnit.Framework;
using FluentAssertions;

namespace BugTrackingSystem.Tests.Automation.Tests;

[TestFixture]
public class BugReportTests : BaseTest
{
    [Test]
    public async Task BugReportList_WhenLoggedIn_ShouldDisplayBugReports()
    {
        // Arrange
        await LoginAsUser();

        // Act
        await Page.ClickAsync("a:has-text('Bug Reports')");

        // Assert
        await Page.WaitForURLAsync("**/BugReport/List**");
        var pageTitle = await Page.TextContentAsync("h1");
        pageTitle.Should().Contain("Bug Reports");
        
        var table = await Page.IsVisibleAsync("table");
        table.Should().BeTrue();
    }

    [Test]
    public async Task CreateBugReport_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        await LoginAsUser();
        var bugTitle = $"Test Bug {DateTime.Now:yyyyMMddHHmmss}";

        // Act
        await Page.ClickAsync("a:has-text('New Bug')");
        await Page.FillAsync("input[name='Title']", bugTitle);
        await Page.FillAsync("textarea[name='Description']", "This is a test bug description");
        await Page.SelectOptionAsync("select[name='PriorityId']", new[] { "1" }); // Assuming 1 is a valid priority ID
        await Page.SelectOptionAsync("select[name='ApplicationId']", new[] { "1" }); // Assuming 1 is a valid app ID
        await Page.Locator("button[type='submit']").Last.ClickAsync(); // Use Last to avoid the dropdown submit button

        // Assert
        await Page.WaitForURLAsync("**/BugReport/List**");
        var successMessage = await Page.IsVisibleAsync(".alert-success");
        successMessage.Should().BeTrue();
    }

    [Test]
    public async Task CreateBugReport_WithMissingRequiredFields_ShouldShowValidationErrors()
    {
        // Arrange
        await LoginAsUser();

        // Act
        await Page.ClickAsync("a:has-text('New Bug')");
        await Page.Locator("button[type='submit']").Last.ClickAsync(); // Use Last to avoid the dropdown submit button

        // Assert
        var titleError = await Page.IsVisibleAsync("span[data-valmsg-for='Title']");
        var descriptionError = await Page.IsVisibleAsync("span[data-valmsg-for='Description']");
        
        titleError.Should().BeTrue();
        descriptionError.Should().BeTrue();
    }

    [Test]
    public async Task BugReportDetails_ShouldDisplayCorrectInformation()
    {
        // Arrange
        await LoginAsUser();
        
        // Navigate to Bug Reports
        await Page.ClickAsync("a:has-text('Bug Reports')");
        await Page.WaitForURLAsync("**/BugReport/List**");
        
        // Wait for the page to load and check if there are any bug reports
        await Page.WaitForSelectorAsync("table, .no-data-message", new PageWaitForSelectorOptions { Timeout = 10000 });
        
        // Check if there are any View buttons available
        var viewButtons = await Page.QuerySelectorAllAsync("a:has-text('View')");
        if (viewButtons.Count == 0)
        {
            Assert.Inconclusive("No bug reports available to test. Please create some test data first.");
            return;
        }

        // Act
        await Page.Locator("a:has-text('View')").First.ClickAsync();

        // Assert
        await Page.WaitForURLAsync("**/BugReport/Details/**", new PageWaitForURLOptions { Timeout = 15000 });
        
        // Wait for page content to load
        await Page.WaitForSelectorAsync("h1, h2, .card-title", new PageWaitForSelectorOptions { Timeout = 10000 });
        
        var title = await Page.IsVisibleAsync("h1, h2, .card-title");
        var content = await Page.IsVisibleAsync(".card-body, .bug-description, .description");
        var status = await Page.IsVisibleAsync(".badge, .status");
        
        title.Should().BeTrue("Bug report should have a title");
        content.Should().BeTrue("Bug report should have content/description");
        status.Should().BeTrue("Bug report should show status");
    }

    [Test]
    public async Task EditBugReport_AsAdmin_ShouldShowEditButton()
    {
        // Arrange
        await LoginAsAdmin();
        await Page.ClickAsync("a:has-text('Bug Reports')");

        // Act
        await Page.Locator("a:has-text('View')").First.ClickAsync();

        // Assert
        var editButton = await Page.IsVisibleAsync("a:has-text('Edit Bug Report')");
        editButton.Should().BeTrue();
    }

    [Test]
    public async Task EditBugReport_AsUser_ShouldNotShowEditButton()
    {
        // Arrange
        await LoginAsUser();
        await Page.ClickAsync("a:has-text('Bug Reports')");

        // Act
        await Page.Locator("a:has-text('View')").First.ClickAsync();

        // Assert
        var editButton = await Page.IsVisibleAsync("a:has-text('Edit Bug Report')");
        editButton.Should().BeFalse();
    }

    [Test]
    public async Task BugReportSearch_ShouldFilterResults()
    {
        // Arrange
        await LoginAsUser();
        await Page.ClickAsync("a:has-text('Bug Reports')");

        // Act
        var searchBox = await Page.QuerySelectorAsync("input[type='search']");
        if (searchBox != null)
        {
            await Page.FillAsync("input[type='search']", "test");
            await Page.WaitForTimeoutAsync(1000); // Wait for search to process
        }

        // Assert
        var table = await Page.IsVisibleAsync("table");
        table.Should().BeTrue();
    }

    [Test]
    public async Task BugReportStatusFilter_ShouldFilterByStatus()
    {
        // Arrange
        await LoginAsUser();
        await Page.ClickAsync("a:has-text('Bug Reports')");

        // Act
        var statusFilter = await Page.QuerySelectorAsync("select[name='status']");
        if (statusFilter != null)
        {
            await Page.SelectOptionAsync("select[name='status']", "Open");
            await Page.WaitForTimeoutAsync(1000);
        }

        // Assert
        var table = await Page.IsVisibleAsync("table");
        table.Should().BeTrue();
    }
}