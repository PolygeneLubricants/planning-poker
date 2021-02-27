using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Engine.Core.Models;

namespace PlanningPoker.Engine.Core.Managers
{
    internal static class ServerManager
    {
        internal static Player AddPlayer(PokerServer server, string playerPrivateId, string playerName, PlayerType type)
        {
            var publicId = GeneratePublicId(server.Players);
            var player = new Player(playerPrivateId, publicId, playerName, type);
            server.Players.Add(playerPrivateId, player);
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

        internal static void RemovePlayer(PokerServer server, string playerPrivateId)
        {
            var player = server.Players[playerPrivateId];
            server.Players.Remove(playerPrivateId);
            SessionManager.RemovePlayer(server.CurrentSession, player.PublicId);
        }

        internal static IList<PokerServer> RemovePlayerFromAllServers(IEnumerable<PokerServer> servers, string playerId)
        {
            var serversWithUser = servers.Where(s => s.Players.ContainsKey(playerId)).ToList();
            foreach (var server in serversWithUser)
            {
                RemovePlayer(server, playerId);
            }

            return serversWithUser;
        }

        internal static bool TryRemovePlayer(PokerServer server, int playerPublicId, out Player? removedPlayer)
        {
            var player = server.Players.Where(kvp => kvp.Value.PublicId == playerPublicId).Select(kvp => kvp.Value).FirstOrDefault();
            if (player != null)
            {
                RemovePlayer(server, player.Id);
                removedPlayer = player;
                return true;
            }

            removedPlayer = null;
            return false;
        }

        internal static Player GetPlayer(PokerServer server, string playerPrivateId)
        {
            var player = server.Players[playerPrivateId];
            return player;
        }
    }
}