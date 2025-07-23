using Microsoft.Playwright;
using NUnit.Framework;
using FluentAssertions;

namespace BugTrackingSystem.Tests.Automation.Tests;

[TestFixture]
public class ConnectivityTest : BaseTest
{
    [Test]
    public async Task Application_ShouldBeAccessible()
    {
        // This test just verifies that the application is running and accessible
        
        // Act
        await Page.GotoAsync(BaseUrl);
        
        // Assert
        var title = await Page.TitleAsync();
        title.Should().Contain("Bug Tracking System", "Application should be accessible and return the correct title");
        
        // Verify the page loaded properly
        var body = await Page.IsVisibleAsync("body");
        body.Should().BeTrue("Page body should be visible");
        
        Console.WriteLine($"✅ Successfully connected to: {BaseUrl}");
        Console.WriteLine($"✅ Page title: {title}");
    }

    [Test]
    public async Task LoginPage_ShouldBeAccessible()
    {
        // Act
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        
        // Assert
        var emailField = await Page.IsVisibleAsync("input[name='Input.Email']");
        var passwordField = await Page.IsVisibleAsync("input[name='Input.Password']");
        var loginButton = await Page.IsVisibleAsync("button[type='submit']");
        
        emailField.Should().BeTrue("Email field should be visible");
        passwordField.Should().BeTrue("Password field should be visible");
        loginButton.Should().BeTrue("Login button should be visible");
        
        Console.WriteLine("✅ Login page is accessible and has required fields");
    }

    [Test]
    public async Task UserLogin_ShouldWork()
    {
        // This test specifically tests the login functionality
        try
        {
            await LoginAsUser();
            
            // Verify we're logged in
            var isLoggedIn = await IsLoggedIn();
            isLoggedIn.Should().BeTrue("User should be logged in successfully");
            
            Console.WriteLine("✅ User login test passed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ User login test failed: {ex.Message}");
            
            // Take screenshot for debugging
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "login-test-failure.png" });
            
            // Print current URL and page content for debugging
            Console.WriteLine($"Current URL: {Page.Url}");
            var pageContent = await Page.ContentAsync();
            Console.WriteLine($"Page content length: {pageContent.Length}");
            
            throw;
        }
    }
}