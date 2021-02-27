using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PlanningPoker.Server;
using PlanningPoker.Server.Infrastructure.HostedServices;

namespace PlanningPoker.FunctionalTests
{
    public class PlanningPokerWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return
                WebHost.CreateDefaultBuilder()
                    .UseEnvironment("LocalDevelopment")
                    .UseStartup<Startup>()
                    .UseTestServer();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureTestServices(services =>
            {
                // Remove automatic services, so these don't influence integration tests
                // that could be dependent on the outcome of these.
                services.Remove(services.Single(s => s.ImplementationType == typeof(CleanupServerJob)));
                services.AddSingleton<CleanupServerJob>();
            });
        }
    }
}