using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Core;
using PlanningPoker.Core.Models;
using PlanningPoker.Core.Utilities;
using PlanningPoker.Server.ViewModelMappers;
using PlanningPoker.Shared;
using PlanningPoker.Shared.ViewModels;

namespace PlanningPoker.Server.Hubs
{
    public class PlanningPokerHub : Hub
    {
        private readonly IServerStore _serverStore;
        private readonly IDateTimeProvider _dateTimeProvider;

        public PlanningPokerHub(
            IServerStore serverStore, 
            IDateTimeProvider dateTimeProvider)
        {
            _serverStore = serverStore;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task Connect(Guid id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, id.ToString());
        }

        public async Task Kick(Guid id, string playerId, int playerPublicIdToRemove)
        {
            var server = _serverStore.Get(id);
            var player = PokerServerManager.GetPlayer(server, playerId);
            var wasRemoved = PokerServerManager.TryRemovePlayer(server, playerPublicIdToRemove, out var removedPlayer);
            if (wasRemoved && removedPlayer != null)
            {
                await Clients.Group(server.Id.ToString()).SendAsync(Messages.KICKED, removedPlayer.Map(false));
                await Groups.RemoveFromGroupAsync(removedPlayer.Id, id.ToString());
                await Clients.Group(server.Id.ToString()).SendAsync(Messages.UPDATED, server.Map());
                await BroadcastLog(id.ToString(), player.Name, $"Kicked {removedPlayer.Name}.");
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var serversWithPlayer = _serverStore.RemovePlayerFromAllServers(Context.ConnectionId);
            foreach (var server in serversWithPlayer)
            {
                await Clients.Group(server.Id.ToString()).SendAsync(Messages.UPDATED, server.Map());
            }
            await base.OnDisconnectedAsync(exception);
        }

        public ServerCreationResult Create(string desiredCardSet)
        {
            var isParsed = CardSetProcessor.TryParseCardSet(desiredCardSet, out var cardSet, out var validationMessage);
            var creationResult = new ServerCreationResult
            {
                ValidationMessage = validationMessage
            };

            if (isParsed)
            {
                var server = _serverStore.Create(cardSet);
                creationResult.ServerId = server.Id;
                creationResult.Created = true;
            }

            creationResult.ValidationMessage = validationMessage;
            return creationResult;
        }

        public async Task<PlayerViewModel> Join(Guid id, string playerName, PlayerType type)
        {
            var server = _serverStore.Get(id);
            var newPlayer = PokerServerManager.AddPlayer(server, Context.ConnectionId, playerName, type);
            await Clients.Group(id.ToString()).SendAsync(Messages.UPDATED, server.Map());
            await BroadcastLog(id.ToString(), playerName, "Joined the server.");
            return newPlayer.Map(includePrivateId: true);
        }

        public async Task Vote(Guid serverId, string playerId, string vote)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CardSet.Contains(vote)) return;
            if (!server.CurrentSession.CanVote) return;
            if (!server.Players.ContainsKey(playerId)) return;

            var player = PokerServerManager.GetPlayer(server, playerId);
            if (player.Type == PlayerType.Observer) return;

            PokerSessionEngine.SetVote(server.CurrentSession, player.PublicId, vote);
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
            await BroadcastLog(serverId.ToString(), player.Name, "Voted.");
        }

        public async Task UnVote(Guid serverId, string playerId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanVote) return;
            if (!server.Players.ContainsKey(playerId)) return;

            var player = PokerServerManager.GetPlayer(server, playerId);
            if (player.Type == PlayerType.Observer) return;

            PokerSessionEngine.RemoveVote(server.CurrentSession, player.PublicId);
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
            await BroadcastLog(serverId.ToString(), player.Name, "Redacted their vote.");
        }

        public async Task Clear(Guid serverId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanClear) return;

            PokerSessionEngine.Clear(server.CurrentSession);
            var player = PokerServerManager.GetPlayer(server, Context.ConnectionId);
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
            await Clients.Group(serverId.ToString()).SendAsync(Messages.CLEAR);
            await BroadcastLog(serverId.ToString(), player.Name, "Cleared all votes.");
        }

        public async Task Show(Guid serverId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanShow(server.Players)) return;

            PokerSessionEngine.Show(server.CurrentSession);
            var player = PokerServerManager.GetPlayer(server, Context.ConnectionId);
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
            await BroadcastLog(serverId.ToString(), player.Name, "Made all votes visible.");
        }

        public async Task BroadcastLog(string serverId, string user, string message)
        {
            var now = _dateTimeProvider.GetUtcNow();
            var logMessage = new LogMessage
            {
                User = user,
                Message = message,
                Timestamp = now
            };

            await Clients.Group(serverId).SendAsync(Messages.LOG, logMessage);
        }
    }
}