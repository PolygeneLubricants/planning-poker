using System;
using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Core.Models;

namespace PlanningPoker.Core
{
    public interface IServerStore
    {
        PokerServer Create(IList<string> cardSet);

        PokerServer Get(Guid id);

        IList<PokerServer> RemovePlayerFromAllServers(string playerId);

        ICollection<PokerServer> All();

        void Remove(PokerServer server);
    }

    public class ServerStore : IServerStore
    {
        private readonly IDictionary<Guid, PokerServer> _servers;

        public ServerStore()
        {
            _servers = new Dictionary<Guid, PokerServer>();
        }

        public PokerServer Create(IList<string> cardSet)
        {
            var newServerId = Guid.NewGuid();
            var newServer = new PokerServer(newServerId, cardSet);
            _servers.Add(newServerId, newServer);
            return newServer;
        }

        public PokerServer Get(Guid id)
        {
            return _servers[id];
        }

        public IList<PokerServer> RemovePlayerFromAllServers(string playerId)
        {
            var serversWithUser = _servers.Where(s => s.Value.Players.ContainsKey(playerId)).Select(pair => pair.Value).ToList();
            foreach (var server in serversWithUser)
            {
                PokerServerManager.RemovePlayer(server, playerId);
            }

            return serversWithUser;
        }
        
        public ICollection<PokerServer> All()
        {
            return _servers.Values;
        }

        public void Remove(PokerServer server)
        {
            _servers.Remove(server.Id);
        }
    }
}