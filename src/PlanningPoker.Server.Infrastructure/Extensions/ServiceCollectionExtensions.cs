using Microsoft.Extensions.DependencyInjection;
using PlanningPoker.Engine.Core;
using PlanningPoker.Server.Infrastructure.HostedServices;

namespace PlanningPoker.Server.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IServerStore, ServerStore>();
            services.AddSingleton<IPlanningPokerEngine, PlanningPokerEngine>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddHostedService<CleanupServerJob>();

            return services;
        }
    }
}