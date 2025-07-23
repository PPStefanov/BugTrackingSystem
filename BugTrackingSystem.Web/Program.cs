using BugTrackingSystem.Data;
using BugTrackingSystem.Models.Entities;
using BugTrackingSystem.Services.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<BugTrackingSystemDbContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<BugTrackingSystemDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
.AddEntityFrameworkStores<BugTrackingSystemDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

// Configure application cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddControllersWithViews(options =>
{
    // Add anti-forgery token validation globally
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddRazorPages();

// Add anti-forgery services
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});
builder.Services.AddScoped<IBugReportService, BugReportService>();
builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Configure custom error pages
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            
            // Add global exception handling
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("An error occurred. Please try again later.");
                });
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();



            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.SeedRolesAsync(services);
    await SeedData.SeedUsersAsync(services); // <--- ADD THIS LINE!
    await SeedData.SeedPrioritiesAsync(services);
    await SeedData.SeedApplicationsAsync(services); 
    await SeedData.SeedStatusesAsync(services);
}



app.Run();