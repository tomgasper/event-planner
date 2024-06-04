using EventPlanner.Data;
using EventPlanner.Models;
using EventPlanner.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.AddEventLog(eventLogSettings =>
{
    eventLogSettings.SourceName = "EventPlanner";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IEventService, EventService>();

// Configure Identity .NET
builder.Services.AddIdentity<AppUser, AppUserRole>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 3;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<EventPlannerDbContext>();

builder.Services.AddTransient<IdentitySetupService>();

var connectionString = builder.Configuration.GetConnectionString("AppData");
builder.Services.AddDbContext<EventPlannerDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Create roles and super user
using (var scope = app.Services.CreateScope())
{
    var identitySetupService = scope.ServiceProvider.GetRequiredService<IdentitySetupService>();
    await identitySetupService.CreateRolesAsync();
    await identitySetupService.CreateAdmin();
}

app.Run();
