using Microsoft.Extensions.DependencyInjection;
using PlanningPoker.Core;
using PlanningPoker.Core.Utilities;

namespace PlanningPoker.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IServerStore, ServerStore>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}