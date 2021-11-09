using Analogy.LogServer.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

namespace Analogy.LogServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            ServiceConfiguration serviceConfiguration = new ServiceConfiguration();
            Configuration.Bind("ServiceConfiguration", serviceConfiguration);
            services.AddSingleton(serviceConfiguration);
            services.AddSingleton<GRPCLogConsumer>();
            services.AddGrpc();
            services.AddSingleton<MessagesContainer>();
            services.AddSingleton<MessageHistoryContainer>();
            services.AddSingleton<WindowsEventLogsMonitor>();

            //services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MessagesContainer container, IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(async () =>
            {
                container.Stop();
                await OnShutdown();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }

        private async Task OnShutdown()
        {
            await GrpcEnvironment.ShutdownChannelsAsync();
            await GrpcEnvironment.KillServersAsync();
        }
    }
}
