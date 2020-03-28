using System.Linq;
using PlanningPoker.Core.Models;

namespace PlanningPoker.Core
{
    public static class PokerServerManager
    {
        public static Player AddPlayer(PokerServer server, string playerName)
        {
            var nextId = server.Players.Any() ? server.Players.Max(p => p.Id) + 1 : 0;
            var player = new Player(nextId, playerName);
            server.Players.Add(player);
            return player;
        }
    }
}