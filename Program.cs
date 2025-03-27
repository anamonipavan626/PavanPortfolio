using Microsoft.AspNetCore.DataProtection;
using PavanPortfolio;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;

var builder = WebApplication.CreateBuilder(args); 
builder.WebHost.UseUrls("http://+:8080");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/app/DataProtection-Keys"))
    .SetApplicationName("PavanPortfolio");
// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddControllersWithViews();

var CronSchedule = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, false).Build().GetSection("APIKeys:CronSchedule").Value;

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IJobFactory, JobFactory>();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddHostedService<QuartzHostedService>();
builder.Services.AddSingleton<FDSchedular>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton(new JobSchedule(
    jobType: typeof(FDSchedular),
    cronExpression: "0 0/14 * * * ?"));

var specificOrgins = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: specificOrgins,
                      policy =>
                      {
                          policy.AllowAnyOrigin();
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                      });
});

builder.WebHost.ConfigureKestrel(c =>
{
    c.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
});
var app = builder.Build();
 

app.UseExceptionHandler("/Registration/Error");
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Portfolio}/{action=PortFolio}/{id?}"); 
app.Run();
