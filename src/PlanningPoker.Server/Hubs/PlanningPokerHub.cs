using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Core;
using PlanningPoker.Core.Extensions;
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
            var player = server.GetPlayer(playerId);
            var wasRemoved = server.TryRemovePlayer(playerPublicIdToRemove, out var removedPlayer);
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

        public Guid Create()
        {
            var server = _serverStore.Create();
            return server.Id;
        }

        public async Task<PlayerViewModel> Join(Guid id, string playerName, PlayerType type)
        {
            var server = _serverStore.Get(id);
            var newPlayer = server.Join(Context.ConnectionId, playerName, type);
            await Clients.Group(id.ToString()).SendAsync(Messages.UPDATED, server.Map());
            await BroadcastLog(id.ToString(), playerName, "Joined the server.");
            return newPlayer.Map(includePrivateId: true);
        }

        public async Task Vote(Guid serverId, string playerId, int vote)
        {
            if (!Cards.Values.Contains(vote)) return;

            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanVote) return;
            if (!server.Players.ContainsKey(playerId)) return;

            var player = server.GetPlayer(playerId);
            if (player.Type == PlayerType.Observer) return;

            server.CurrentSession.Vote(player.PublicId, vote);
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
            await BroadcastLog(serverId.ToString(), player.Name, "Voted.");
        }

        public async Task UnVote(Guid serverId, string playerId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanVote) return;
            if (!server.Players.ContainsKey(playerId)) return;

            var player = server.GetPlayer(playerId);
            if (player.Type == PlayerType.Observer) return;

            server.CurrentSession.UnVote(player.PublicId);
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
            await BroadcastLog(serverId.ToString(), player.Name, "Redacted their vote.");
        }

        public async Task Clear(Guid serverId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanClear) return;

            server.CurrentSession.Clear();
            var player = server.GetPlayer(Context.ConnectionId);
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
            await Clients.Group(serverId.ToString()).SendAsync(Messages.CLEAR);
            await BroadcastLog(serverId.ToString(), player.Name, "Cleared all votes.");
        }

        public async Task Show(Guid serverId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanShow(server.Players)) return;

            server.CurrentSession.Show();
            var player = server.GetPlayer(Context.ConnectionId);
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