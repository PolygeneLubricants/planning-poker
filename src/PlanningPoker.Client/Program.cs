using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanningPoker.Client.Configuration;
using PlanningPoker.Client.Storage;

namespace PlanningPoker.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddBlazoredLocalStorage();
            RegisterDependencies(builder.Services, builder.Configuration);
            builder.RootComponents.Add<App>("app");
            await builder.Build().RunAsync();
        }

        private static IServiceCollection RegisterDependencies(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IServerSessionManager, ServerSessionManager>();
            services.AddTransient<IUserPreferencesManager, UserPreferencesManager>();
            services.Configure<NextEnvironmentConfiguration>(configuration.GetSection(NextEnvironmentConfiguration.Path));
            return services;
        }
    }
}