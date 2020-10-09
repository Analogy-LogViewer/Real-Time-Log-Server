using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Analogy.LogServer.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Analogy.LogServer
{
    public class Program
    {
        public static async Task Main()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .AddJsonFile("appsettings_LogServer.json").Build();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

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

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder().UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
               {
                   var config = new ConfigurationBuilder()
                       .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))  //location of the exe file
                       .AddJsonFile("appsettings_LogServer.json", optional: false, reloadOnChange: true).Build();
                   webBuilder.UseConfiguration(config)
                       .ConfigureKestrel((context, options) =>
                       {
                           options.Configure(context.Configuration.GetSection("Kestrel"))
                               .Endpoint("Https", listenOptions =>
                               {
                                   listenOptions.ListenOptions.Protocols = HttpProtocols.Http2;
                               });
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
