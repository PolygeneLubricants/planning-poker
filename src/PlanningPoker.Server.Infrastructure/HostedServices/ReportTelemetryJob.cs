using Microsoft.Extensions.Hosting;
using PlanningPoker.Engine.Core;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;

namespace PlanningPoker.Server.Infrastructure.HostedServices
{
    public class ReportTelemetryJob : IHostedService, IDisposable
    {
        private Timer? _timer;
        private static readonly TimeSpan RunFrequency = TimeSpan.FromMinutes(10);
        private readonly IServerStore _serverStore;
        private readonly TelemetryClient _telemetryClient;
        private const string ReportPlayersMetricName  = "players";
        private const string ReportSessionsMetricName = "sessions";

        public ReportTelemetryJob(IServerStore serverStore, TelemetryClient telemetryClient)
        {
            _serverStore = serverStore;
            _telemetryClient = telemetryClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(_ => RunJob(), null, TimeSpan.Zero, RunFrequency);
            return Task.CompletedTask;
        }

        private void RunJob()
        {
            ReportSessions();
            ReportPlayers();
        }

        private void ReportSessions()
        {
            var allPlayers = _serverStore.All().Count;
            _telemetryClient.TrackMetric(ReportSessionsMetricName, allPlayers);
        }

        private void ReportPlayers()
        {
            var allPlayers = _serverStore.All().Sum(server => server.Players.Count);
            _telemetryClient.TrackMetric(ReportPlayersMetricName, allPlayers);
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
