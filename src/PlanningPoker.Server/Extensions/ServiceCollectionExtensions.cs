using Microsoft.Extensions.DependencyInjection;
using PlanningPoker.Core;

namespace PlanningPoker.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IServerStore, ServerStore>();

            return services;
        }
    }
}