using System;
using System.Collections.Generic;
using PlanningPoker.Core.Models;

namespace PlanningPoker.Core
{
    public interface IServerStore
    {
        PokerServer Create();

        PokerServer Get(Guid id);
    }

    public class ServerStore : IServerStore
    {
        private readonly IDictionary<Guid, PokerServer> _servers;

        public ServerStore()
        {
            _servers = new Dictionary<Guid, PokerServer>();
        }

        public PokerServer Create()
        {
            var newServerId = Guid.NewGuid();
            var newServer = new PokerServer(newServerId);
            _servers.Add(newServerId, newServer);
            return newServer;
        }

        public PokerServer Get(Guid id)
        {
            return _servers[id];
        }
    }
}