global using dabrelCMS.code;
global using dabrelCMS.data;
global using dabrelCMS.models;
global using Microsoft.EntityFrameworkCore;
global using System;
using dabrelCMS;

var builder = WebApplication.CreateBuilder(args);

CMSConfig.ConStr = builder.Configuration.GetConnectionString("DefaultConnection");
CMSConfig.JwtKey = builder.Configuration["Jwt:Key"];

// Add services to the container
//builder.Services.AddDbContext<CMSDbContext>(options => options.UseSqlite(CMSConfig.ConStr));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
//app.UseCors();
//app.UseResponseCaching();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCMSMiddleware();

app.Run();