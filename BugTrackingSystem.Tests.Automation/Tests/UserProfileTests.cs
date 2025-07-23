using Microsoft.Playwright;
using NUnit.Framework;
using FluentAssertions;

namespace BugTrackingSystem.Tests.Automation.Tests;

[TestFixture]
public class UserProfileTests : BaseTest
{
    [Test]
    public async Task UserProfile_AccessFromDropdown_ShouldDisplayProfilePage()
    {
        // Arrange
        await LoginAsUser();

        // Act
        await Page.ClickAsync(".dropdown-toggle");
        await Page.ClickAsync("a:has-text('Profile Settings')");

        // Assert
        await Page.WaitForURLAsync("**/UserProfile**");
        var pageTitle = await Page.TextContentAsync("h4");
        pageTitle.Should().Contain("User Profile");
    }

    [Test]
    public async Task UserProfile_DisplaysCorrectUserInformation()
    {
        // Arrange
        await LoginAsUser();

        // Act
        await Page.ClickAsync(".dropdown-toggle");
        await Page.ClickAsync("a:has-text('Profile Settings')");

        // Assert
        var username = await Page.TextContentAsync("p.form-control-plaintext:nth-of-type(1)");
        var email = await Page.TextContentAsync("p.form-control-plaintext:nth-of-type(2)");
        var role = await Page.TextContentAsync(".badge");

        username.Should().Contain("qa@demo.com");
        email.Should().Contain("qa@demo.com");
        role.Should().Contain("User");
    }

    [Test]
    public async Task ChangePassword_WithValidData_ShouldShowSuccessMessage()
    {
        // Arrange
        await LoginAsUser();
        await Page.ClickAsync(".dropdown-toggle");
        await Page.ClickAsync("a:has-text('Profile Settings')");

        // Act
        await Page.FillAsync("input[name='CurrentPassword']", UserPassword);
        await Page.FillAsync("input[name='NewPassword']", "NewPassword123!");
        await Page.FillAsync("input[name='ConfirmNewPassword']", "NewPassword123!");
        await Page.ClickAsync("button:has-text('Change Password')");

        // Assert
        await Page.WaitForSelectorAsync(".alert-success");
        var successMessage = await Page.TextContentAsync(".alert-success");
        successMessage.Should().Contain("Password changed successfully!");

        // Cleanup - change password back
        await Page.FillAsync("input[name='CurrentPassword']", "NewPassword123!");
        await Page.FillAsync("input[name='NewPassword']", UserPassword);
        await Page.FillAsync("input[name='ConfirmNewPassword']", UserPassword);
        await Page.ClickAsync("button:has-text('Change Password')");
    }

    [Test]
    public async Task ChangePassword_WithIncorrectCurrentPassword_ShouldShowError()
    {
        // Arrange
        await LoginAsUser();
        await Page.ClickAsync(".dropdown-toggle");
        await Page.ClickAsync("a:has-text('Profile Settings')");

        // Act
        await Page.FillAsync("input[name='CurrentPassword']", "WrongPassword123!");
        await Page.FillAsync("input[name='NewPassword']", "NewPassword123!");
        await Page.FillAsync("input[name='ConfirmNewPassword']", "NewPassword123!");
        await Page.ClickAsync("button:has-text('Change Password')");

        // Assert
        var errorExists = await Page.IsVisibleAsync(".text-danger");
        errorExists.Should().BeTrue();
    }

    [Test]
    public async Task ChangePassword_WithMismatchedPasswords_ShouldShowValidationError()
    {
        // Arrange
        await LoginAsUser();
        await Page.ClickAsync(".dropdown-toggle");
        await Page.ClickAsync("a:has-text('Profile Settings')");

        // Act
        await Page.FillAsync("input[name='CurrentPassword']", UserPassword);
        await Page.FillAsync("input[name='NewPassword']", "NewPassword123!");
        await Page.FillAsync("input[name='ConfirmNewPassword']", "DifferentPassword123!");
        await Page.ClickAsync("button:has-text('Change Password')");

        // Assert
        var validationError = await Page.TextContentAsync("span[data-valmsg-for='ConfirmNewPassword']");
        validationError.Should().Contain("do not match");
    }

    [Test]
    public async Task ChangePassword_WithWeakPassword_ShouldShowValidationError()
    {
        // Arrange
        await LoginAsUser();
        await Page.ClickAsync(".dropdown-toggle");
        await Page.ClickAsync("a:has-text('Profile Settings')");

        // Act
        await Page.FillAsync("input[name='CurrentPassword']", UserPassword);
        await Page.FillAsync("input[name='NewPassword']", "123");
        await Page.FillAsync("input[name='ConfirmNewPassword']", "123");
        await Page.ClickAsync("button:has-text('Change Password')");

        // Assert
        var validationError = await Page.IsVisibleAsync("span[data-valmsg-for='NewPassword']");
        validationError.Should().BeTrue();
    }

    [Test]
    public async Task UserProfile_BackToDashboard_ShouldNavigateCorrectly()
    {
        // Arrange
        await LoginAsUser();
        await Page.ClickAsync(".dropdown-toggle");
        await Page.ClickAsync("a:has-text('Profile Settings')");

        // Act
        await Page.ClickAsync("a:has-text('Back to Dashboard')");

        // Assert
        await Page.WaitForURLAsync($"{BaseUrl}/");
        var pageTitle = await Page.TextContentAsync("h1.page-title");
        pageTitle.Should().Contain("Dashboard");
    }

    [Test]
    public async Task UserProfile_AccessWithoutLogin_ShouldRedirectToLogin()
    {
        // Act
        await Page.GotoAsync($"{BaseUrl}/UserProfile");

        // Assert
        await Page.WaitForURLAsync("**/Identity/Account/Login**");
        var currentUrl = Page.Url;
        currentUrl.Should().Contain("/Identity/Account/Login");
    }
}