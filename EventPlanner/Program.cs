using EventPlanner.Data;
using EventPlanner.Models;
using EventPlanner.Services;
using EventPlanner.Interfaces;
using Microsoft.EntityFrameworkCore;

using EventPlanner.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.AddEventLog(eventLogSettings =>
{
    eventLogSettings.SourceName = "EventPlanner";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IDbContext, EventPlannerDbContext>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ILoginHistoryService, LoginHistoryService>();
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection("FileStorageSettings"));

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

app.UseExceptionHandler("/Error/Index");

/*
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Index");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
*/

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Index",
    pattern: "{controller=Event}/{id?}",
    defaults: new { controller = "Event", action = "Index"} ) ;

// Create roles and super user
using (var scope = app.Services.CreateScope())
{
    var identitySetupService = scope.ServiceProvider.GetRequiredService<IdentitySetupService>();
    await identitySetupService.CreateRolesAsync();
    await identitySetupService.CreateAdmin();
}

app.Run();
