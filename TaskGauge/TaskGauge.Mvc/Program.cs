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
