using System.Threading.Tasks;
using Analogy.LogServer.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
namespace Analogy.LogServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;
        public void ConfigureServices(IServiceCollection services)
        {

            CommonSystemConfiguration serviceConfiguration = new CommonSystemConfiguration();
            Configuration.Bind("ServiceConfiguration", serviceConfiguration);
            services.AddSingleton(serviceConfiguration);
            services.AddSingleton<GRPCLogConsumer>();
            services.AddGrpc();
            services.AddSingleton<MessagesContainer>();
            //services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MessagesContainer container, IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(async ()=>
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
                //endpoints.MapHealthChecks("/health");
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
