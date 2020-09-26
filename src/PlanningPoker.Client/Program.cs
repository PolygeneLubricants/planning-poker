using System.Threading.Tasks;
using Blazored.Localisation;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PlanningPoker.Client.Storage;

namespace PlanningPoker.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddBlazoredLocalisation();
            builder.Services.AddBlazoredSessionStorage();
            RegisterDependencies(builder.Services);
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }

        private static IServiceCollection RegisterDependencies(IServiceCollection services)
        {
            services.AddTransient<IServerSessionManager, ServerSessionManager>();
            return services;
        }
    }
}