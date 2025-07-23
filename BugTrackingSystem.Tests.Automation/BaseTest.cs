using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace BugTrackingSystem.Tests.Automation;

[TestFixture]
public class BaseTest : PageTest
{
    protected const string BaseUrl = "https://localhost:7001"; // Update with your actual URL
    protected const string AdminEmail = "admin@demo.com";
    protected const string AdminPassword = "Admin123!";
    protected const string UserEmail = "qa@demo.com";
    protected const string UserPassword = "Qa123!";

    [SetUp]
    public async Task Setup()
    {
        // Set default timeout
        Page.SetDefaultTimeout(30000);
        
        // Navigate to the application
        await Page.GotoAsync(BaseUrl);
    }

    protected async Task LoginAsAdmin()
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        await Page.FillAsync("input[name='Input.Email']", AdminEmail);
        await Page.FillAsync("input[name='Input.Password']", AdminPassword);
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForURLAsync($"{BaseUrl}/");
    }

    protected async Task LoginAsUser()
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        await Page.FillAsync("input[name='Input.Email']", UserEmail);
        await Page.FillAsync("input[name='Input.Password']", UserPassword);
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForURLAsync($"{BaseUrl}/");
    }

    protected async Task Logout()
    {
        // Click on user dropdown
        await Page.ClickAsync(".dropdown-toggle");
        
        // Click logout
        await Page.ClickAsync("button:has-text('Logout')");
        
        // Wait for redirect to home page
        await Page.WaitForURLAsync($"{BaseUrl}/");
    }

    protected async Task WaitForPageLoad()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}