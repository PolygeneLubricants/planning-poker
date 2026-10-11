using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlanningPoker.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlanningPoker.Server.Infrastructure.HostedServices
{
    /// <summary>
    /// Periodically reports anonymous, aggregated usage counts to CloudWatch.
    /// Disabled unless "Metrics:Enabled" is true. Credentials and region are
    /// resolved by the AWS SDK (on EC2: the instance's IAM role).
    /// </summary>
    public class ReportTelemetryJob : IHostedService, IDisposable
    {
        private Timer? _timer;
        private static readonly TimeSpan RunFrequency = TimeSpan.FromMinutes(5);
        private const string Namespace = "PlanningPoker";
        private readonly IServerStore _serverStore;
        private readonly IUsageCounters _usageCounters;
        private readonly ILogger<ReportTelemetryJob> _logger;
        private readonly bool _enabled;
        private IAmazonCloudWatch? _cloudWatch;

        public ReportTelemetryJob(
            IServerStore serverStore,
            IUsageCounters usageCounters,
            IConfiguration configuration,
            ILogger<ReportTelemetryJob> logger)
        {
            _serverStore = serverStore;
            _usageCounters = usageCounters;
            _logger = logger;
            _enabled = configuration.GetValue<bool>("Metrics:Enabled");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_enabled) return Task.CompletedTask;

            _cloudWatch = new AmazonCloudWatchClient();
            _timer = new Timer(_ => _ = RunJob(), null, RunFrequency, RunFrequency);
            return Task.CompletedTask;
        }

        private async Task RunJob()
        {
            var usage = _usageCounters.ReadAndReset();
            var servers = _serverStore.All();
            var now = DateTime.UtcNow;

            var data = new List<MetricDatum>
            {
                Datum("RoomsCreated", usage.RoomsCreated, now),
                Datum("PlayersJoined", usage.PlayersJoined, now),
                Datum("RoundsPlayed", usage.RoundsPlayed, now),
                Datum("ActiveRooms", servers.Count, now),
                Datum("ActivePlayers", servers.Sum(server => server.Players.Count), now),
            };

            try
            {
                await _cloudWatch!.PutMetricDataAsync(new PutMetricDataRequest
                {
                    Namespace = Namespace,
                    MetricData = data
                });
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to report usage metrics to CloudWatch.");
            }
        }

        private static MetricDatum Datum(string name, double value, DateTime timestamp)
        {
            return new MetricDatum
            {
                MetricName = name,
                Value = value,
                Unit = StandardUnit.Count,
                TimestampUtc = timestamp
            };
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
                _cloudWatch?.Dispose();
            }
        }
    }
}
