using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using PlanningPoker.Hub.Client;
using PlanningPoker.Hub.Client.Abstractions;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public class PlanningPokerHubTestBuilder
    {
        private const string HubBaseAddress = "/hubs/poker";
        private HubConnection _hubConnection;

        public PlanningPokerHubTestBuilder(PlanningPokerWebApplicationFactory factory)
        {
            _hubConnection = CreateConnection(factory);
            _hubConnection.StartAsync().GetAwaiter().GetResult();
            HubClient = new PlanningPokerHubClient(_hubConnection);
            HubClient.OnConnected(() => Task.CompletedTask);
        }

        public IPlanningPokerHubClient HubClient { get; }

        private HubConnection CreateConnection(PlanningPokerWebApplicationFactory factory)
        {
            var client = factory.CreateClient();
            var hubUri = new Uri(client.BaseAddress, HubBaseAddress);
            var connection = new HubConnectionBuilder()
                .WithUrl(hubUri, o =>
                {
                    o.Transports = HttpTransportType.LongPolling;
                    o.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler();
                })
                .Build();
            return connection;
        }

        public PlanningPokerHubTestBuilder WithServer(out Guid serverId)
        {
            const string validCardSet = "1,2,3,5,8,13,21,?";
            return WithServer(validCardSet, out serverId);
        }

        public PlanningPokerHubTestBuilder WithServer(string cardSet, out Guid serverId)
        {
            var result = HubClient.CreateServer(cardSet).GetAwaiter().GetResult();
            serverId = result.ServerId ?? throw new ArgumentNullException(nameof(result.ServerId));
            return this;
        }

        public PlanningPokerHubTestBuilder WithPlayer(Guid serverId, out PlayerViewModel player)
        {
            return WithPlayer(serverId, Guid.NewGuid(), PlayerType.Participant, Guid.NewGuid().ToString(), out player);
        }

        public PlanningPokerHubTestBuilder WithObserver(Guid serverId, out PlayerViewModel player)
        {
            return WithPlayer(serverId, Guid.NewGuid(), PlayerType.Observer, Guid.NewGuid().ToString(), out player);
        }

        public PlanningPokerHubTestBuilder WithPlayerVoted(Guid serverId, string playerPrivateId, string vote)
        {
            AwaitEventResult<PokerServerViewModel>(() =>
                        HubClient.Vote(serverId, playerPrivateId, vote),
                    HubClient.OnSessionUpdated)
                .GetAwaiter().GetResult();
            return this;
        }

        public PlanningPokerHubTestBuilder WithPlayerUnVoted(Guid serverId, string playerPrivateId)
        {
            AwaitEventResult<PokerServerViewModel>(() =>
                        HubClient.UnVote(serverId, playerPrivateId),
                    HubClient.OnSessionUpdated)
                .GetAwaiter().GetResult();
            return this;
        }

        public PlanningPokerHubTestBuilder WithVotesShown(Guid serverId)
        {
            AwaitEventResult<PokerServerViewModel>(() =>
                        HubClient.ShowVotes(serverId),
                    HubClient.OnSessionUpdated)
                .GetAwaiter().GetResult();
            return this;
        }

        public PlanningPokerHubTestBuilder WithVotesCleared(Guid serverId)
        {
            AwaitEventResult(() =>
                        HubClient.ClearVotes(serverId),
                    HubClient.OnVotesCleared)
                .GetAwaiter().GetResult();
            return this;
        }

        public PlanningPokerHubTestBuilder WithPlayerKicked(Guid serverId, string initiatingPlayerPrivateId, int kickedPlayerPublicId)
        {
            AwaitEventResult<PlayerViewModel>(() => 
                HubClient.KickPlayer(serverId, initiatingPlayerPrivateId, kickedPlayerPublicId), 
                HubClient.OnPlayerKicked)
                .GetAwaiter().GetResult();
            return this;
        }

        private PlanningPokerHubTestBuilder WithPlayer(Guid serverId, Guid recoveryId, PlayerType playerType, string playerName, out PlayerViewModel player)
        {
            HubClient.Connect(serverId).GetAwaiter().GetResult();
            player = AwaitEventResult<PokerServerViewModel, PlayerViewModel>(
                () => HubClient.JoinServer(serverId, recoveryId, playerName, playerType),
                HubClient.OnSessionUpdated).GetAwaiter().GetResult();
            return this;
        }

        private async Task AwaitEventResult(Func<Task> taskToAwait, Action<Action> onEvent)
        {
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            onEvent(() =>
            {
                awaitResponse.Release();
            });

            await taskToAwait();
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
        }

        private async Task AwaitEventResult<T>(Func<Task> taskToAwait, Action<Action<T>> onEvent)
        {
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            onEvent(_ =>
            {
                awaitResponse.Release();
            });

            await taskToAwait();
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
        }

        private async Task<TResult> AwaitEventResult<T, TResult>(Func<Task<TResult>> taskToAwait, Action<Action<T>> onEvent)
        {
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            onEvent(_ =>
            {
                awaitResponse.Release();
            });

            var taskResult = await taskToAwait();
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            return taskResult;
        }
    }
}