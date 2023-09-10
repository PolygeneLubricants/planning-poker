using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace PlanningPoker.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddCommandLine(args)
                    .Build())
                .UseStartup<Startup>()

                // Leaving commented code in the codebase is not a good idea,
                // but I think this warrants an exception.
                // Blazor does not allow local debugging where you set the ASPNETCORE_ENVIRONMENT to anything
                // other than Development: https://github.com/dotnet/aspnetcore/issues/43110#issuecomment-1206573745.
                // In order to test environment-specific configuration,
                // the below line has to be added.
                // Note, this line should *not* be added in production code,
                // as it enables locally run code to get static assets from the build output.
                // We don't want that behavior on an environment running on published files.
                //.UseStaticWebAssets() // <-- This is the line to comment in to test environments.
                // End comment block.
                .Build();
        }
    }
}