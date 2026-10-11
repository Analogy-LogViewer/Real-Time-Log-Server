using Analogy.LogServer.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Analogy.LogServer
{
    public class Program
    {
        public static async Task Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            if (!string.IsNullOrEmpty(dir))
            {
                Environment.CurrentDirectory = dir;
            }
            var configuration = new ConfigurationBuilder()
                .SetBasePath(dir ?? Environment.CurrentDirectory)
                .AddJsonFile("appsettings_LogServer.json").Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();
            try
            {
                Log.Information("Starting Analogy Log Server");
                await CreateHostBuilder().Build().RunAsync();
                await GrpcEnvironment.ShutdownChannelsAsync();
                await GrpcEnvironment.KillServersAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Error during application");
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject as Exception, "Error: {e}", e);
        }

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder().UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
                    var config = new ConfigurationBuilder()
                        .SetBasePath(dir ?? Environment.CurrentDirectory)
                        .AddJsonFile("appsettings_LogServer.json", optional: false, reloadOnChange: true).Build();
                    webBuilder.UseConfiguration(config)
                        .ConfigureKestrel((context, options) => options.Configure(context.Configuration.GetSection("Kestrel")))

                        .UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<CleanUpWorker>();
                    services.AddHostedService<WindowsEventLogsMonitor>();
                }).UseWindowsService();
    }
}