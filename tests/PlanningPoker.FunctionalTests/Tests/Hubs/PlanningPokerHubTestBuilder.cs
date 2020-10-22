using System;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using PlanningPoker.Core.Models;
using PlanningPoker.Hub.Client;
using PlanningPoker.Shared.ViewModels;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public class PlanningPokerHubTestBuilder
    {
        private const string HubBaseAddress = "/hubs/poker";

        public PlanningPokerHubTestBuilder(PlanningPokerWebApplicationFactory factory)
        {
            var hubConnection = CreateConnection(factory);
            hubConnection.StartAsync().GetAwaiter().GetResult();
            HubClient = new PlanningPokerHubClient(hubConnection);
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
            var result = HubClient.CreateServer(validCardSet).GetAwaiter().GetResult();
            serverId = result.ServerId ?? throw new ArgumentNullException(nameof(result.ServerId));
            return this;
        }

        public PlanningPokerHubTestBuilder WithPlayer(Guid serverId, out PlayerViewModel player)
        {
            return WithPlayer(serverId, PlayerType.Participant, Guid.NewGuid().ToString(), out player);
        }

        public PlanningPokerHubTestBuilder WithObserver(Guid serverId, out PlayerViewModel player)
        {
            return WithPlayer(serverId, PlayerType.Observer, Guid.NewGuid().ToString(), out player);
        }

        public PlanningPokerHubTestBuilder WithPlayerVoted(Guid serverId, string playerPrivateId, string vote)
        {
            HubClient.Vote(serverId, playerPrivateId, vote).GetAwaiter().GetResult();
            return this;
        }

        private PlanningPokerHubTestBuilder WithPlayer(Guid serverId, PlayerType playerType, string playerName, out PlayerViewModel player)
        {
            HubClient.Connect(serverId).GetAwaiter().GetResult();
            player = HubClient.JoinServer(serverId, playerName, playerType).GetAwaiter().GetResult();
            return this;
        }
    }
}