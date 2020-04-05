using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Core.Extensions;
using PlanningPoker.Core.Models;

namespace PlanningPoker.Core
{
    public static class PokerServerManager
    {
        public static Player AddPlayer(PokerServer server, string playerId, string playerName, PlayerType type)
        {
            var publicId = GeneratePublicId(server.Players);
            var player = new Player(playerId, publicId, playerName, type);
            server.Players.Add(playerId, player);
            return player;
        }

        private static int GeneratePublicId(IDictionary<string, Player> serverPlayers)
        {
            var isEmpty = serverPlayers?.Count == 0;
            if (isEmpty)
            {
                return 0;
            }
            else
            {
                var highestId = serverPlayers.Max(p => p.Value.PublicId);
                return ++highestId;
            }
        }

        public static void RemovePlayer(PokerServer server, string playerId)
        {
            var player = server.Players[playerId];
            server.Players.Remove(playerId);
            server.CurrentSession.RemovePlayer(player.PublicId);
        }
    }
}