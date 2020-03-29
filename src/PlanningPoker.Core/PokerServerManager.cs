using PlanningPoker.Core.Extensions;
using PlanningPoker.Core.Models;

namespace PlanningPoker.Core
{
    public static class PokerServerManager
    {
        public static Player AddPlayer(PokerServer server, string playerId, string playerName)
        {
            var player = new Player(playerId, playerName);
            server.Players.Add(playerId, player);
            return player;
        }

        public static void RemovePlayer(PokerServer server, string playerId)
        {
            server.Players.Remove(playerId);
            server.CurrentSession.RemovePlayer(playerId);
        }
    }
}