using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PlanningPoker.Core.Models;
using PlanningPoker.Shared;
using PlanningPoker.Shared.ViewModels;

namespace PlanningPoker.Hub.Client
{
    public interface IPlanningPokerHubClient
    {
        string? ConnectionId { get; }

        Task Connect(Guid serverId);

        Task ClearVotes(Guid serverId);

        Task<ServerCreationResult> CreateServer(string cardSet);

        Task<PlayerViewModel> JoinServer(Guid serverId, string playerName, PlayerType playerType);

        Task KickPlayer(Guid serverId, string initiatingPlayerPrivateId, int kickedPlayerPublicId);

        Task ShowVotes(Guid serverId);

        Task UnVote(Guid serverId, string playerPrivateId);

        Task Vote(Guid serverId, string playerPrivateId, string vote);

        void OnSessionUpdated(Action<PokerServerViewModel> onSessionUpdatedHandler);

        void OnPlayerKicked(Action<PlayerViewModel> onPlayerKickedHandler);

        void OnLogMessageReceived(Action<LogMessage> onLogMessageReceivedHandler);

        void OnVotesCleared(Action onVotesClearedHandler);
    }

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

        public void OnSessionUpdated(Action<PokerServerViewModel> onSessionUpdatedHandler)
        {
            _hubConnection.On(Messages.UPDATED, onSessionUpdatedHandler);
        }

        public void OnPlayerKicked(Action<PlayerViewModel> onPlayerKickedHandler)
        {
            _hubConnection.On(Messages.KICKED, onPlayerKickedHandler);
        }

        public void OnLogMessageReceived(Action<LogMessage> onLogMessageReceivedHandler)
        {
            _hubConnection.On(Messages.LOG, onLogMessageReceivedHandler);
        }

        public void OnVotesCleared(Action onVotesClearedHandler)
        {
            _hubConnection.On(Messages.CLEAR, onVotesClearedHandler);
        }
    }
}