using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Task_FileMigration;

var builder = new ConfigurationBuilder();

builder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
var logName = Path.Combine(logDirectory, $"log_{DateTime.Now:yyyyMMddHHmmss}.log");

if (!Directory.Exists(logDirectory))
{
    Directory.CreateDirectory(logDirectory);
}

Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .WriteTo.Console()
                .WriteTo.File(logName)
                .CreateLogger();

Log.Logger.Information("starting serilog in a console app...");

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) => {
        // here we can build our services...
        services.AddSingleton<IConfiguration>(builder.Build());
        services.AddSingleton<ILogger>(Log.Logger);
    })
.UseSerilog()
.Build();

var startup = host.Services.GetRequiredService<Startup>();

startup.Run();