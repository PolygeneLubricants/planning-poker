using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PlanningPoker.Engine.Core;
using PlanningPoker.Engine.Core.Models;

namespace PlanningPoker.Server.Infrastructure
{
    public class ServerStore : IServerStore
    {
        private readonly IDictionary<Guid, PokerServer> _servers;

        public ServerStore()
        {
            _servers = new ConcurrentDictionary<Guid, PokerServer>();
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