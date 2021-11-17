using System;
using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Engine.Core.Models;

namespace PlanningPoker.Engine.Core.Managers
{
    internal static class ServerManager
    {
        internal static Player AddOrUpdatePlayer(PokerServer server, Guid recoveryId, string playerPrivateId, string playerName, PlayerType type)
        {
            Player player;
            
            if (server.Players.Any(p => p.Value.RecoveryId == recoveryId))
            {
                // When a player disconnects and reconnects, the connection id / private ID can change.
                // Therefore, the caller sends along a recovery ID to recover / change the id to the new connection id.
                RecoverPlayer(server, recoveryId, playerPrivateId);
                player = WakePlayer(server, playerPrivateId);
            }
            else
            {
                var publicId = GeneratePublicId(server.Players);
                player = new Player(playerPrivateId, recoveryId, publicId, playerName, type);
                server.Players[playerPrivateId] = player;
            }

            return player;
        }

        private static void RecoverPlayer(PokerServer server, Guid recoveryId, string newPlayerPrivateId)
        {
            var playerWithRecoveryId = server.Players.FirstOrDefault(p => p.Value?.RecoveryId == recoveryId).Value;
            if (playerWithRecoveryId == null)
            {
                return;
            }

            server.Players.Remove(playerWithRecoveryId.Id);
            playerWithRecoveryId.Id = newPlayerPrivateId;
            server.Players.Add(playerWithRecoveryId.Id, playerWithRecoveryId);
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

        internal static IList<PokerServer> SetPlayerToSleepOnAllServers(IEnumerable<PokerServer> servers, string playerPrivateId)
        {
            var serversWithUser = servers.Where(s => s.Players.ContainsKey(playerPrivateId)).ToList();
            foreach (var server in serversWithUser)
            {
                SleepPlayer(server, playerPrivateId);
            }

            return serversWithUser;
        }

        internal static Player SleepPlayer(PokerServer server, string playerPrivateId)
        {
            return SetPlayerMode(server, playerPrivateId, PlayerMode.Asleep);
        }

        internal static Player WakePlayer(PokerServer server, string playerPrivateId)
        {
            return SetPlayerMode(server, playerPrivateId, PlayerMode.Awake);
        }
        
        private static Player SetPlayerMode(PokerServer server, string playerPrivateId, PlayerMode mode)
        {
            var player = GetPlayer(server, playerPrivateId);
            player.Mode = mode;
            return player;
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

        public static Player ChangePlayerType(PokerServer server, Player player, PlayerType newType)
        {
            player.Type = newType;
            return player;
        }
    }
}