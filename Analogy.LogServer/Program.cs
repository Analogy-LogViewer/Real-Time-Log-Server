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
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
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
                Log.CloseAndFlush();
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
                   var config = new ConfigurationBuilder()
                       .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))  //location of the exe file
                       .AddJsonFile("appsettings_LogServer.json", optional: false, reloadOnChange: true).Build();
                   webBuilder.UseConfiguration(config)
                       .ConfigureKestrel((context, options) =>
                       {
                           options.Configure(context.Configuration.GetSection("Kestrel"));
                       })

                       .UseStartup<Startup>();
               })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<CleanUpWorker>();
                    services.AddHostedService<WindowsEventLogsMonitor>();
                }).UseWindowsService();
    }
}