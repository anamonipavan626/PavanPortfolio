using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args); 
builder.WebHost.UseUrls("http://+:8080");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/app/DataProtection-Keys"))
    .SetApplicationName("PavanPortfolio");
// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Portfolio}/{action=PortFolio}/{id?}"); 
app.Run();
