using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegistrationSystem.Api.Data;
using RegistrationSystem.Api.Data.Models;
using RegistrationSystem.Api.Helpers;
using RegistrationSystem.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
connectionString = connectionString.Replace("%DataDirectory%", Path.GetFullPath("Data"));
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<DbUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.Name = "RegistrationSystem";
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(1440);
    options.SlidingExpiration = true;
    options.LoginPath = "/api/Account/Login";
    options.AccessDeniedPath = "/api/Account/AccessDenied";
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.ConsentCookie.IsEssential = true;
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.IsEssential = true;
        options.Cookie.HttpOnly = false;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.None;
        options.LoginPath = "/api/Account/Login";
        options.LogoutPath = "/api/Account/Logout";
    });

builder.SetupLogging();

builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseCookiePolicy();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseSession();

app.HandleUnhandledExceptions<Program>();

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    SetupIdentitiesAsync(scope).Wait();
}

app.Run();

public partial class Program
{
    internal static async Task SetupIdentitiesAsync(IServiceScope scope)
    {
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        context!.Database.EnsureCreated();
        var userService = scope.ServiceProvider.GetService<UserService>();
        await userService!.RegisterRolesIfDoesntExistsAsync();
        await userService!.RegisterAdminIfDoesntExistsAsync();
    }
}
