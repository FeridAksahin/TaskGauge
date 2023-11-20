using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using TaskGauge.DataAccessLayer.Concrete;
using TaskGauge.DataAccessLayer.Interface;
using TaskGauge.Entity.Context;
using TaskGauge.Mvc.Hubs;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "TaskGauge.Authentication.Cookie";
    options.LoginPath = "/Login/Index";
    options.AccessDeniedPath = "/Error/NotFound";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddMvc(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});


builder.Services.AddControllersWithViews();

var connectionString = configuration.GetConnectionString("ConnectionStringForTaskGauge");
builder.Services.AddDbContext<TaskGaugeContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddScoped<TaskGaugeContext>();
builder.Services.AddScoped<IUserDal, UserDal>();


builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

 
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<TaskGaugeHub>("/taskGaugeHub");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=Index}/{id?}");
});

app.Run();
