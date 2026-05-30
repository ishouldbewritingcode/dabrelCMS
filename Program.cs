global using dabrelCMS.code;
global using dabrelCMS.data;
global using dabrelCMS.models;
global using Microsoft.EntityFrameworkCore;
global using System;
using dabrelCMS;
using Serilog;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.Enrich.FromLogContext()
	.CreateLogger();

builder.Host.UseSerilog();

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

// Get logger for error handling
var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.Use(async (context, next) =>
{
	try
	{
		await next.Invoke();
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "Unhandled exception in request");
		throw;
	}
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCMSUploaderMiddleware();

app.UseCMSMiddleware();

var testLogger = app.Services.GetRequiredService<ILogger<Program>>();
testLogger.LogInformation("===== APPLICATION STARTED - Serilog is working! =====");

app.Run();
