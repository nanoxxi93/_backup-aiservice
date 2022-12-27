using AIService.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace aiservice.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = CreateHostBuilder(args).Build();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.cdnsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.extrasettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.logsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.watsonsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("sqlfunctions.json", optional: true, reloadOnChange: true)
                .Build();
            AppSettings _appSettings = configuration.Get<AppSettings>();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Async(a =>
                {
                    a.Map("Layer", "Other", (name, wt) =>
                    {
                        string rootPath = _appSettings.LogRootPath ?? "logs";
                        wt.File(Path.Combine(rootPath, $"{DateTime.Now:yyyyMMdd}.{name}.log"),
                            retainedFileCountLimit: null,
                            rollOnFileSizeLimit: true,
                            outputTemplate: "{Timestamp:yyyyMMdd.HHmmss zzz} [{Level:u3}] {Ip} {Guid} {MethodName} --> {Message:lj}{NewLine}{Exception}",
                            shared: true);
                        if (_appSettings.LogDNASettings != null && _appSettings.LogDNASettings.Enabled)
                        {
                            wt.LogDNA(apiKey: _appSettings.LogDNASettings.IngestionKey, appName: "aiservice");
                        }
                    });
                })
                .CreateLogger();
            webHost.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) => {
                //config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile("appsettings.cdnsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile("appsettings.extrasettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile("appsettings.logsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile("appsettings.watsonsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile("sqlfunctions.json", optional: true, reloadOnChange: true);
            })
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseIISIntegration();
                webBuilder.UseUrls("http://*:62859");
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Limits.MaxRequestBodySize = long.MaxValue;
                });
                webBuilder.UseStartup<Startup>();
            });
    }
}
