using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Core;
using PlanningPoker.Core.Extensions;
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

        public Guid Create()
        {
            var server = _serverStore.Create();
            return server.Id;
        }

        public async Task<PlayerViewModel> Join(Guid id, string playerName)
        {
            var server = _serverStore.Get(id);
            var newPlayer = server.Join(playerName);
            await Clients.Group(id.ToString()).SendAsync(Messages.UPDATED, server.Map());
            return newPlayer.Map();
        }

        public async Task Vote(Guid serverId, int playerId, int vote)
        {
            var server = _serverStore.Get(serverId);
            server.CurrentSession.Vote(playerId, vote);
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
        }

        public async Task Clear(Guid serverId)
        {
            var server = _serverStore.Get(serverId);
            server.CurrentSession.Clear();
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
        }

        public async Task Show(Guid serverId)
        {
            var server = _serverStore.Get(serverId);
            server.CurrentSession.Show();
            await Clients.Group(serverId.ToString()).SendAsync(Messages.UPDATED, server.Map());
        }
    }
}