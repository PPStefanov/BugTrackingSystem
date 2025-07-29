using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace BugTrackingSystem.Tests.Automation;

[TestFixture]
public class BaseTest : PageTest
{
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions()
        {
            ViewportSize = new ViewportSize() { Width = 1280, Height = 720 },
            IgnoreHTTPSErrors = true
        };
    }

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        // Force headed mode by setting environment variable
        Environment.SetEnvironmentVariable("HEADED", "1");
    }

    protected static readonly string BaseUrl = Environment.GetEnvironmentVariable("TEST_BASE_URL") ?? "https://localhost:44384";
    protected const string AdminEmail = "admin@demo.com";
    protected const string AdminPassword = "123456";
    protected const string UserEmail = "qa@demo.com";
    protected const string UserPassword = "123456";

    [SetUp]
    public async Task Setup()
    {
        // Set longer timeout for slower operations
        Page.SetDefaultTimeout(60000); // Increased to 60 seconds
        
        // Make browser actions slower for better visibility
        Page.SetDefaultNavigationTimeout(30000);
        
        // Navigate to the application and wait for it to load
        await Page.GotoAsync(BaseUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
        
        // Wait a bit for any JavaScript to initialize
        await Page.WaitForTimeoutAsync(2000); // Increased wait time for better visibility
    }

    protected async Task LoginAsAdmin()
    {
        try
        {
            Console.WriteLine("Attempting to login as Admin...");
            await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            
            // Wait for login form to be visible
            await Page.WaitForSelectorAsync("input[name='Input.Email']", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            
            await Page.FillAsync("input[name='Input.Email']", AdminEmail);
            await Page.FillAsync("input[name='Input.Password']", AdminPassword);
            
            // Click login and wait for navigation
            await Page.ClickAsync("button[type='submit']");
            
            // Wait for successful login - look for user greeting or dashboard
            await Page.WaitForSelectorAsync(".user-greeting, .page-title", new PageWaitForSelectorOptions { Timeout = 30000 });
            
            Console.WriteLine("Admin login successful");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Admin login failed: {ex.Message}");
            // Take screenshot for debugging
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "admin-login-failure.png" });
            throw;
        }
    }

    protected async Task LoginAsUser()
    {
        try
        {
            Console.WriteLine("Attempting to login as User...");
            await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            
            // Wait for login form to be visible
            await Page.WaitForSelectorAsync("input[name='Input.Email']", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            
            await Page.FillAsync("input[name='Input.Email']", UserEmail);
            await Page.FillAsync("input[name='Input.Password']", UserPassword);
            
            // Click login and wait for navigation
            await Page.ClickAsync("button[type='submit']");
            
            // Wait for successful login - look for user greeting or dashboard
            await Page.WaitForSelectorAsync(".user-greeting, .page-title", new PageWaitForSelectorOptions { Timeout = 30000 });
            
            Console.WriteLine("User login successful");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"User login failed: {ex.Message}");
            // Take screenshot for debugging
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "user-login-failure.png" });
            throw;
        }
    }

    protected async Task Logout()
    {
        try
        {
            // Wait for dropdown to be available
            await Page.WaitForSelectorAsync(".dropdown-toggle", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            
            // Click on user dropdown
            await Page.ClickAsync(".dropdown-toggle");
            
            // Wait for dropdown menu to appear
            await Page.WaitForSelectorAsync("button:has-text('Logout')", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            
            // Click logout
            await Page.ClickAsync("button:has-text('Logout')");
            
            // Wait for redirect to home page or login page
            await Page.WaitForSelectorAsync("a:has-text('Login'), .user-greeting", new PageWaitForSelectorOptions { Timeout = 15000 });
            
            Console.WriteLine("Logout successful");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logout failed: {ex.Message}");
            throw;
        }
    }

    protected async Task WaitForPageLoad()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(500); // Small additional wait for any animations
    }

    protected async Task<bool> IsLoggedIn()
    {
        try
        {
            return await Page.IsVisibleAsync(".user-greeting");
        }
        catch
        {
            return false;
        }
    }

    protected async Task EnsureLoggedOut()
    {
        if (await IsLoggedIn())
        {
            await Logout();
        }
    }
}