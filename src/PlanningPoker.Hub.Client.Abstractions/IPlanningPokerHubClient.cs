using System;
using System.Threading.Tasks;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;

namespace PlanningPoker.Hub.Client.Abstractions
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
}