using Application;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.DbInitializer;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register IEmailSender as Scoped
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register IDbInitializer
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// Retrieve Stripe settings from the database
builder.Services.AddSingleton<StripeSettings>(sp =>
{
    using (var scope = sp.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // Retrieve the StripeSettings from your database
        var stripeSettings = dbContext.StripeSettings.FirstOrDefault();
        if (stripeSettings == null)
        {
            throw new Exception("Stripe settings not found in the database.");
        }
        return stripeSettings;
    }
});

// Configure Stripe API with settings from the database
builder.Services.AddSingleton(sp =>
{
    var stripeSettings = sp.GetRequiredService<StripeSettings>();
    StripeConfiguration.ApiKey = stripeSettings.SecretKey;
    return new StripeClient(stripeSettings.SecretKey);
});

// Register background services properly
builder.Services.AddHostedService<TripReviewService>();
builder.Services.AddHostedService<TripService>();
builder.Services.AddHostedService<BookingService>();
builder.Services.AddScoped<NotificationService>();

// Register SignalR
builder.Services.AddSignalR()
    .AddHubOptions<NotificationHub>(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromMinutes(10);
    });


builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();

});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register Razor Pages and HTTP Client
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

// Configure Identity Options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Views/Shared/Error.cshtml");
    app.UseHsts();
}

//Seed DB using custom IDbInitializer
//SeedDatabase();


// Map SignalR hub to the correct endpoint
app.MapHub<NotificationHub>("/notificationHub");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=User}/{controller=Home}/{action=Index}/{id?}");
app.Run();
