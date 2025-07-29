using Microsoft.Playwright;
using NUnit.Framework;
using FluentAssertions;

namespace BugTrackingSystem.Tests.Automation.Tests;

[TestFixture]
public class NavigationTests : BaseTest
{
    [Test]
    public async Task Sidebar_Navigation_ShouldWorkCorrectly()
    {
        // Arrange
        await LoginAsUser();

        // Test Dashboard navigation
        await Page.ClickAsync("a:has-text('Dashboard')");
        await Page.WaitForURLAsync($"{BaseUrl}/");
        var dashboardTitle = await Page.TextContentAsync("h1.page-title");
        dashboardTitle.Should().Contain("Dashboard");

        // Test Bug Reports navigation
        await Page.ClickAsync("a:has-text('Bug Reports')");
        await Page.WaitForURLAsync("**/BugReport/List**");
        var bugReportsTitle = await Page.TextContentAsync("h1");
        bugReportsTitle.Should().Contain("Bug Reports");

        // Test New Bug navigation
        await Page.ClickAsync("a:has-text('New Bug')");
        await Page.WaitForURLAsync("**/BugReport/Form**");
        var newBugTitle = await Page.TextContentAsync("h1");
        newBugTitle.Should().Contain("Create Bug Report");

        // Test Users navigation
        await Page.ClickAsync("a:has-text('Users')");
        await Page.WaitForURLAsync("**/User/List**");
        var usersTitle = await Page.TextContentAsync("h1");
        usersTitle.Should().Contain("Users");
    }

    [Test]
    public async Task AdminNavigation_ShouldShowAdminOnlyLinks()
    {
        // Arrange
        await LoginAsAdmin();

        // Act & Assert
        var applicationsLink = await Page.IsVisibleAsync("a:has-text('Applications')");
        var reportsLink = await Page.IsVisibleAsync("a:has-text('Reports')");
        
        applicationsLink.Should().BeTrue();
        reportsLink.Should().BeTrue();

        // Test Applications navigation
        await Page.ClickAsync("a:has-text('Applications')");
        await Page.WaitForURLAsync("**/Applications/List**");

        // Test Reports navigation
        await Page.ClickAsync("a:has-text('Reports')");
        await Page.WaitForURLAsync("**/Reports**");
    }

    [Test]
    public async Task UserNavigation_ShouldHideAdminOnlyLinks()
    {
        // Arrange
        await LoginAsUser();

        // Act & Assert
        var applicationsLink = await Page.IsVisibleAsync("a:has-text('Applications')");
        var reportsLink = await Page.IsVisibleAsync("a:has-text('Reports')");
        
        applicationsLink.Should().BeFalse();
        reportsLink.Should().BeFalse();
    }

    [Test]
    public async Task SidebarToggle_ShouldWorkCorrectly()
    {
        // Arrange
        await LoginAsUser();

        // Act
        await Page.ClickAsync("#sidebar-toggle");
        await Page.WaitForTimeoutAsync(500); // Wait for animation

        // Assert
        var sidebarClass = await Page.GetAttributeAsync("#sidebar", "class");
        sidebarClass.Should().Contain("collapsed");

        // Toggle back
        await Page.ClickAsync("#sidebar-toggle");
        await Page.WaitForTimeoutAsync(500);
        
        sidebarClass = await Page.GetAttributeAsync("#sidebar", "class");
        sidebarClass.Should().NotContain("collapsed");
    }

    [Test]
    public async Task BreadcrumbNavigation_ShouldDisplayCorrectly()
    {
        // Arrange
        await LoginAsUser();

        // Act
        await Page.ClickAsync("a:has-text('Bug Reports')");
        await Page.Locator("a:has-text('View')").First.ClickAsync();

        // Assert
        var breadcrumb = await Page.IsVisibleAsync(".breadcrumb");
        if (breadcrumb)
        {
            var breadcrumbText = await Page.TextContentAsync(".breadcrumb");
            breadcrumbText.Should().Contain("Bug Reports");
        }
    }

    [Test]
    public async Task UserDropdown_ShouldShowCorrectOptions()
    {
        // Arrange
        await LoginAsUser();

        // Act
        await Page.ClickAsync(".dropdown-toggle");

        // Assert
        var profileLink = await Page.IsVisibleAsync("a:has-text('Profile Settings')");
        var logoutButton = await Page.IsVisibleAsync("button:has-text('Logout')");
        
        profileLink.Should().BeTrue();
        logoutButton.Should().BeTrue();
    }

    [Test]
    public async Task NotificationButton_ShouldBeVisible()
    {
        // Arrange
        await LoginAsUser();

        // Act & Assert
        var notificationButton = await Page.IsVisibleAsync("#notification-btn");
        notificationButton.Should().BeTrue();
    }

    [Test]
    public async Task ResponsiveDesign_ShouldWorkOnMobile()
    {
        // Arrange
        await Page.SetViewportSizeAsync(375, 667); // iPhone size
        await LoginAsUser();

        // Act & Assert
        var sidebar = await Page.IsVisibleAsync("#sidebar");
        var toggleButton = await Page.IsVisibleAsync("#sidebar-toggle");
        
        sidebar.Should().BeTrue();
        toggleButton.Should().BeTrue();

        // Test mobile navigation
        await Page.ClickAsync("#sidebar-toggle");
        await Page.WaitForTimeoutAsync(500);
        
        var sidebarClass = await Page.GetAttributeAsync("#sidebar", "class");
        sidebarClass.Should().Contain("collapsed");
    }

    [Test]
    public async Task PageTitle_ShouldUpdateCorrectly()
    {
        // Arrange
        await LoginAsUser();

        // Test different pages
        await Page.ClickAsync("a:has-text('Bug Reports')");
        var title1 = await Page.TitleAsync();
        title1.Should().Contain("Bug Tracking System");

        await Page.ClickAsync("a:has-text('New Bug')");
        var title2 = await Page.TitleAsync();
        title2.Should().Contain("Bug Tracking System");
    }

    [Test]
    public async Task Footer_ShouldBeVisible()
    {
        // Arrange
        await LoginAsUser();

        // Act & Assert
        var footer = await Page.IsVisibleAsync("footer");
        footer.Should().BeTrue();
        
        var footerText = await Page.TextContentAsync("footer");
        footerText.Should().Contain("2025 - BugTrackingSystem");
    }
}