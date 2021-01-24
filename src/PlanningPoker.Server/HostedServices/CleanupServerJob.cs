using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PlanningPoker.Engine.Core;

namespace PlanningPoker.Server.HostedServices
{
    public class CleanupServerJob : IHostedService, IDisposable
    {
        private Timer? _timer;
        private readonly IServerStore _serverStore;

        public CleanupServerJob(IServerStore serverStore)
        {
            _serverStore = serverStore;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(_ => RunJob(cancellationToken), null, TimeSpan.Zero, TimeSpan.FromMinutes(20));
            return Task.CompletedTask;
        }

        private void RunJob(CancellationToken cancellationToken)
        {
            var createdThreshold = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));
            foreach (var server in _serverStore.All())
            {
                if (cancellationToken.IsCancellationRequested) break;

                var isOld = createdThreshold > server.Created;
                var isEmpty = !server.Players.Any();
                if (isOld && isEmpty)
                {
                    _serverStore.Remove(server);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Dispose();
            }
        }
    }
}
