using Microsoft.EntityFrameworkCore;
using MvcNetCoreChollometro.Data;
using MvcNetCoreChollometro.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<RepositoryChollometro>();

builder.Services.AddDbContext<ChollometroContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlAzure")));

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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chollos}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
