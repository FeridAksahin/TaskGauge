using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using TaskGauge.Common;
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


builder.Services
    .AddCors(options =>
     options.AddDefaultPolicy(builder =>
     builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.AddMvc(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var connectionString = configuration.GetConnectionString("ConnectionStringForTaskGauge");
builder.Services.AddDbContext<TaskGaugeContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddScoped<TaskGaugeContext>();
builder.Services.AddScoped<IUserDal, UserDal>();
builder.Services.AddScoped<IRoomDal, RoomDal>();
builder.Services.AddScoped<UserInformation>();

builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();

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
