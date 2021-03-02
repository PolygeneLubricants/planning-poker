using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PlanningPoker.Hub.Client.Abstractions;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;

namespace PlanningPoker.Hub.Client
{
    public class PlanningPokerHubClient : IPlanningPokerHubClient
    {
        private readonly HubConnection _hubConnection;

        public PlanningPokerHubClient(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public string? ConnectionId => _hubConnection.ConnectionId;

        public Task Connect(Guid serverId)
        {
            return _hubConnection.InvokeAsync(HubEndpointRoutes.Connect, serverId);
        }

        public Task ClearVotes(Guid serverId)
        {
            return _hubConnection.InvokeAsync(HubEndpointRoutes.Clear, serverId);
        }

        public Task<ServerCreationResult> CreateServer(string cardSet)
        {
            return _hubConnection.InvokeAsync<ServerCreationResult>(HubEndpointRoutes.Create, cardSet);
        }

        public Task<PlayerViewModel> JoinServer(Guid serverId, string playerName, PlayerType playerType)
        {
            return _hubConnection.InvokeAsync<PlayerViewModel>(HubEndpointRoutes.Join, serverId, playerName, playerType);
        }

        public Task KickPlayer(Guid serverId, string initiatingPlayerPublicId, int kickedPlayerPublicId)
        {
            return _hubConnection.InvokeAsync(HubEndpointRoutes.Kick, serverId, initiatingPlayerPublicId, kickedPlayerPublicId);
        }

        public Task ShowVotes(Guid serverId)
        {
            return _hubConnection.InvokeAsync(HubEndpointRoutes.Show, serverId);
        }

        public Task UnVote(Guid serverId, string playerPrivateId)
        {
            return _hubConnection.InvokeAsync(HubEndpointRoutes.UnVote, serverId, playerPrivateId);
        }

        public Task Vote(Guid serverId, string playerPrivateId, string vote)
        {
            return _hubConnection.InvokeAsync(HubEndpointRoutes.Vote, serverId, playerPrivateId, vote);
        }

        public Task<PlayerViewModel> ChangePlayerType(Guid serverId, PlayerType newType)
        {
            return _hubConnection.InvokeAsync<PlayerViewModel>(HubEndpointRoutes.ChangePlayerType, serverId, newType);
        }

        public void OnSessionUpdated(Action<PokerServerViewModel> onSessionUpdatedHandler)
        {
            _hubConnection.On(BroadcastChannels.UPDATED, onSessionUpdatedHandler);
        }

        public void OnPlayerKicked(Action<PlayerViewModel> onPlayerKickedHandler)
        {
            _hubConnection.On(BroadcastChannels.KICKED, onPlayerKickedHandler);
        }

        public void OnLogMessageReceived(Action<LogMessage> onLogMessageReceivedHandler)
        {
            _hubConnection.On(BroadcastChannels.LOG, onLogMessageReceivedHandler);
        }

        public void OnVotesCleared(Action onVotesClearedHandler)
        {
            _hubConnection.On(BroadcastChannels.CLEAR, onVotesClearedHandler);
        }
    }
}