using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlanningPoker.Server.Hubs;
using PlanningPoker.Server.Infrastructure.Extensions;

namespace PlanningPoker.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            services.RegisterDependencies();
            services.AddSingleton<IPlanningPokerEventBroadcaster, PlanningPokerEventBroadcaster>();
            services.AddSignalR(options =>
                {
                    options.EnableDetailedErrors = true;
                })
                .AddJsonProtocol();
            services.AddMvc();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<PlanningPokerHub>("/hubs/poker");
                endpoints.MapFallbackToFile("index.html");
            });

            // Wake up event broadcaster
            _ = app.ApplicationServices.GetRequiredService<IPlanningPokerEventBroadcaster>();
        }
    }
}
