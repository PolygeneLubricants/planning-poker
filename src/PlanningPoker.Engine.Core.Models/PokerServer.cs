using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Engine.Core.Models.Poker;

namespace PlanningPoker.Engine.Core.Models
{
    public class PokerServer
    {
        public PokerServer(Guid id, IList<string> cardSet)
        {
            Id = id;
            Players = new ConcurrentDictionary<string, Player>();
            CurrentSession = new PokerSession(cardSet);
            Created = DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public IDictionary<string, Player> Players { get; set; }

        public PokerSession CurrentSession { get; set; }

        public DateTime Created { get; set; }

        public Player AddOrUpdatePlayer(Guid recoveryId, string playerPrivateId, string playerName, PlayerType type)
        {
            Player player;

            if (Players.Any(p => p.Value.RecoveryId == recoveryId))
            {
                // When a player disconnects and reconnects, the connection id / private ID can change.
                // Therefore, the caller sends along a recovery ID to recover / change the id to the new connection id.
                RecoverPlayer(recoveryId, playerPrivateId);
                player = WakePlayer(playerPrivateId);
            }
            else
            {
                var publicId = GeneratePublicId(Players);
                player = new Player(playerPrivateId, recoveryId, publicId, playerName, type);
                Players[playerPrivateId] = player;
            }

            return player;
        }

        public Player GetPlayer(string playerPrivateId)
        {
            var player = Players[playerPrivateId];
            return player;
        }

        public void RemovePlayer(string playerPrivateId)
        {
            var player = Players[playerPrivateId];
            Players.Remove(playerPrivateId);
            CurrentSession.RemovePlayer(player.PublicId);
        }

        public bool TryRemovePlayer(int playerPublicId, out Player? removedPlayer)
        {
            var player = Players.Where(kvp => kvp.Value.PublicId == playerPublicId).Select(kvp => kvp.Value).FirstOrDefault();
            if (player != null)
            {
                RemovePlayer(player.Id);
                removedPlayer = player;
                return true;
            }

            removedPlayer = null;
            return false;
        }

        public Player ChangePlayerType(Player player, PlayerType newType)
        {
            player.Type = newType;
            return player;
        }

        public Player SleepPlayer(string playerPrivateId)
        {
            return SetPlayerMode(playerPrivateId, PlayerMode.Asleep);
        }

        public Player WakePlayer(string playerPrivateId)
        {
            return SetPlayerMode(playerPrivateId, PlayerMode.Awake);
        }

        private Player SetPlayerMode(string playerPrivateId, PlayerMode mode)
        {
            var player = GetPlayer(playerPrivateId);
            player.Mode = mode;
            return player;
        }

        private void RecoverPlayer(Guid recoveryId, string newPlayerPrivateId)
        {
            var playerWithRecoveryId = Players.FirstOrDefault(p => p.Value?.RecoveryId == recoveryId).Value;
            if (playerWithRecoveryId == null)
            {
                return;
            }

            Players.Remove(playerWithRecoveryId.Id);
            playerWithRecoveryId.Id = newPlayerPrivateId;
            Players.Add(playerWithRecoveryId.Id, playerWithRecoveryId);
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
    }
}