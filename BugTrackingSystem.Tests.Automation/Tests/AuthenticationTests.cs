using Microsoft.Playwright;
using NUnit.Framework;
using FluentAssertions;

namespace BugTrackingSystem.Tests.Automation.Tests;

[TestFixture]
public class AuthenticationTests : BaseTest
{
    [Test]
    public async Task Login_WithValidCredentials_ShouldSucceed()
    {
        // Arrange
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");

        // Act
        await Page.FillAsync("input[name='Input.Email']", UserEmail);
        await Page.FillAsync("input[name='Input.Password']", UserPassword);
        await Page.ClickAsync("button[type='submit']");

        // Assert
        await Page.WaitForURLAsync($"{BaseUrl}/");
        var welcomeText = await Page.TextContentAsync(".user-greeting");
        welcomeText.Should().Contain("Hello qa@demo.com!");
    }

    [Test]
    public async Task Login_WithInvalidCredentials_ShouldShowError()
    {
        // Arrange
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");

        // Act
        await Page.FillAsync("input[name='Input.Email']", "invalid@test.com");
        await Page.FillAsync("input[name='Input.Password']", "wrongpassword");
        await Page.ClickAsync("button[type='submit']");

        // Assert
        var errorMessage = await Page.IsVisibleAsync(".text-danger");
        errorMessage.Should().BeTrue();
    }

    [Test]
    public async Task Logout_WhenLoggedIn_ShouldRedirectToHome()
    {
        // Arrange
        await LoginAsUser();

        // Act
        await Logout();

        // Assert
        await Page.WaitForURLAsync($"{BaseUrl}/");
        var loginButton = await Page.IsVisibleAsync("a:has-text('Login')");
        loginButton.Should().BeTrue();
    }

    [Test]
    public async Task AccessProtectedPage_WithoutLogin_ShouldRedirectToLogin()
    {
        // Act
        await Page.GotoAsync($"{BaseUrl}/BugReport/Form");

        // Assert
        await Page.WaitForURLAsync("**/Identity/Account/Login**");
        var currentUrl = Page.Url;
        currentUrl.Should().Contain("/Identity/Account/Login");
    }

    [Test]
    public async Task AdminAccess_WithUserCredentials_ShouldNotShowAdminFeatures()
    {
        // Arrange
        await LoginAsUser();

        // Act
        await Page.GotoAsync($"{BaseUrl}/");

        // Assert
        var applicationsLink = await Page.IsVisibleAsync("a:has-text('Applications')");
        var reportsLink = await Page.IsVisibleAsync("a:has-text('Reports')");
        
        applicationsLink.Should().BeFalse();
        reportsLink.Should().BeFalse();
    }

    [Test]
    public async Task AdminAccess_WithAdminCredentials_ShouldShowAdminFeatures()
    {
        // Arrange
        await LoginAsAdmin();

        // Act
        await Page.GotoAsync($"{BaseUrl}/");

        // Assert
        var applicationsLink = await Page.IsVisibleAsync("a:has-text('Applications')");
        var reportsLink = await Page.IsVisibleAsync("a:has-text('Reports')");
        
        applicationsLink.Should().BeTrue();
        reportsLink.Should().BeTrue();
    }
}