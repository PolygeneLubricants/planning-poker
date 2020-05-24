using System.Threading.Tasks;
using Blazored.Localisation;
using Microsoft.AspNetCore.Blazor.Hosting;

namespace PlanningPoker.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddBlazoredLocalisation();
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}