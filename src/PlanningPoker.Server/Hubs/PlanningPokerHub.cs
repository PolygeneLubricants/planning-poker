using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Core;
using PlanningPoker.Core.Extensions;
using PlanningPoker.Core.Models;
using PlanningPoker.Server.ViewModelMappers;
using PlanningPoker.Shared;
using PlanningPoker.Shared.ViewModels;

namespace PlanningPoker.Server.Hubs
{
    public class PlanningPokerHub : Hub
    {
        private readonly IServerStore _serverStore;

        public PlanningPokerHub(IServerStore serverStore)
        {
            _serverStore = serverStore;
        }

        public async Task Connect(Guid id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, id.ToString());
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
            return newPlayer.Map(includePrivateId: true);
        }

        public async Task Vote(Guid serverId, string playerId, int vote)
        {
            if (!Cards.Values.Contains(vote)) return;

            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanVote) return;
            if (!server.Players.ContainsKey(playerId)) return;

            var player = server.Players[playerId];
            if (player.Type == PlayerType.Observer) return;

            server.CurrentSession.Vote(player.PublicId, vote);
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
        }

        public async Task UnVote(Guid serverId, string playerId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanVote) return;
            if (!server.Players.ContainsKey(playerId)) return;

            var player = server.Players[playerId];
            if (player.Type == PlayerType.Observer) return;

            server.CurrentSession.UnVote(player.PublicId);
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
        }

        public async Task Clear(Guid serverId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanClear) return;

            server.CurrentSession.Clear();
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
            await Clients.Group(serverId.ToString()).SendAsync(Messages.CLEAR);
        }

        public async Task Show(Guid serverId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanShow(server.Players)) return;

            server.CurrentSession.Show();
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
        }
    }
}