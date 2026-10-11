using System;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        // Make browsers check for updated pages and stylesheets on each load (cheap, thanks to ETags),
        // so a deploy never shows new markup with stale styling.
        private static readonly StaticFileOptions RevalidatedStaticFiles = new StaticFileOptions
        {
            OnPrepareResponse = context =>
            {
                var path = context.File.Name;
                if (path.EndsWith(".html") || path.EndsWith(".css"))
                {
                    context.Context.Response.Headers["Cache-Control"] = "no-cache";
                }
            }
        };

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles(RevalidatedStaticFiles);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<PlanningPokerHub>("/hubs/poker");
                endpoints.MapGet("/environment.js", async context =>
                {
                    var environmentName = JsonSerializer.Serialize(env.EnvironmentName);
                    context.Response.ContentType = "text/javascript";
                    context.Response.Headers["Cache-Control"] = "no-cache";
                    await context.Response.WriteAsync($"window.jppEnvironment = {environmentName};");
                });
                endpoints.MapGet("/health", async context =>
                {
                    var response = new { status = "healthy", timestamp = DateTime.UtcNow };
                    var json = JsonSerializer.Serialize(response);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(json);
                });
                endpoints.MapFallbackToFile("index.html", RevalidatedStaticFiles);
            });

            // Wake up event broadcaster
            _ = app.ApplicationServices.GetRequiredService<IPlanningPokerEventBroadcaster>();
        }
    }
}
