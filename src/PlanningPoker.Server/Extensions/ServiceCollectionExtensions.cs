using Microsoft.Extensions.DependencyInjection;
using PlanningPoker.Engine.Core;
using PlanningPoker.Server.HostedServices;
using PlanningPoker.Server.Hubs;

namespace PlanningPoker.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IServerStore, ServerStore>();
            services.AddSingleton<IPlanningPokerEngine, PlanningPokerEngine>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IPlanningPokerEventBroadcaster, PlanningPokerEventBroadcaster>();
            services.AddHostedService<CleanupServerJob>();

            return services;
        }
    }
}