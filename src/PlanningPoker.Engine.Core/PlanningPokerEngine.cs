using System;
using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Engine.Core.Exceptions;
using PlanningPoker.Engine.Core.Models;
using PlanningPoker.Engine.Core.Models.Events;

namespace PlanningPoker.Engine.Core
{
    public interface IPlanningPokerEngine
    {
        event EventHandler<PlayerKickedEventArgs> PlayerKicked;
        event EventHandler<RoomUpdatedEventArgs> RoomUpdated;
        event EventHandler<LogUpdatedEventArgs> LogUpdated;
        event EventHandler<RoomClearedEventArgs> RoomCleared;

        void Kick(Guid id, string initiatingPlayerPrivateId, int playerPublicIdToRemove);
        void SleepInAllRooms(string playerPrivateId);
        (bool wasCreated, Guid? serverId, string? validationMessages) CreateRoom(string desiredCardSet);
        bool RoomExists(Guid roomId);
        Player JoinRoom(Guid id, Guid recoveryId, string playerName, string playerPrivateId, PlayerType type);
        void Vote(Guid serverId, string playerPrivateId, string vote);
        void RedactVote(Guid serverId, string playerPrivateId);
        void ClearVotes(Guid serverId, string playerPrivateId);
        void ShowVotes(Guid serverId, string playerPrivateId);
        Player ChangePlayerType(Guid serverId, string playerPrivateId, PlayerType newType);
        void SetAutoShow(Guid serverId, string playerPrivateId, bool enabled);
        void SetAdmin(Guid serverId, string initiatingPlayerPrivateId, int playerPublicId, bool isAdmin);
    }

    public class PlanningPokerEngine : IPlanningPokerEngine
    {
        private readonly IServerStore _serverStore;
        private readonly IUsageCounters _usageCounters;
        private const int MaxPlayerNameLength = 20;

        public PlanningPokerEngine(
            IServerStore serverStore,
            IUsageCounters usageCounters)
        {
            _serverStore = serverStore;
            _usageCounters = usageCounters;
        }

        public event EventHandler<PlayerKickedEventArgs> PlayerKicked;
        public event EventHandler<RoomUpdatedEventArgs> RoomUpdated;
        public event EventHandler<LogUpdatedEventArgs> LogUpdated;
        public event EventHandler<RoomClearedEventArgs> RoomCleared;
        
        public void Kick(Guid id, string initiatingPlayerPrivateId, int playerPublicIdToRemove)
        {
            var server = _serverStore.Get(id);
            var player = server.GetPlayer(initiatingPlayerPrivateId);
            EnsureCanControl(server, player);
            var wasRemoved = server.TryRemovePlayer(playerPublicIdToRemove, out var kickedPlayer);
            if (wasRemoved && kickedPlayer != null)
            {
                var autoShown = TryAutoShow(server);
                RaisePlayerKicked(id, kickedPlayer);
                RaiseRoomUpdated(id, server);
                RaiseLogUpdated(id, player.Name, $"Kicked {kickedPlayer.Name}.");
                RaiseAutoShownLogIfApplicable(server, autoShown);
            }
        }

        public void SleepInAllRooms(string playerPrivateId)
        {
            var serversWithPlayer = SetPlayerToSleepOnAllServers(_serverStore.All(), playerPrivateId);
            foreach (var server in serversWithPlayer)
            {
                var autoShown = TryAutoShow(server);
                RaiseRoomUpdated(server.Id, server);
                RaiseAutoShownLogIfApplicable(server, autoShown);
            }
        }

        public (bool wasCreated, Guid? serverId, string? validationMessages) CreateRoom(string desiredCardSet)
        {
            var isParsed = CardSetProcessor.TryParseCardSet(desiredCardSet, out var cardSet, out var validationMessage);
            if (!isParsed)
            {
                return (false, null, validationMessage);
            }

            var server = _serverStore.Create(cardSet);
            _usageCounters.RoomCreated();
            return (true, server.Id, validationMessage);
        }

        public bool RoomExists(Guid roomId)
        {
            return _serverStore.Exists(roomId);
        }

        public Player JoinRoom(Guid id, Guid recoveryId, string playerName, string playerPrivateId, PlayerType type)
        {
            if (string.IsNullOrWhiteSpace(playerName)) throw new MissingPlayerNameException();

            var server = _serverStore.Get(id);
            
            var formattedPlayerName = playerName.Length > MaxPlayerNameLength ? playerName.Substring(0, MaxPlayerNameLength) : playerName;
            var newPlayer = server.AddOrUpdatePlayer(recoveryId, playerPrivateId, formattedPlayerName, type);
            _usageCounters.PlayerJoined();
            RaiseRoomUpdated(id, server);
            RaiseLogUpdated(id, newPlayer.Name, "Joined the server.");
            return newPlayer;
        }

        public void Vote(Guid serverId, string playerPrivateId, string vote)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CardSet.Contains(vote)) throw new VoteException($"Vote does not exist in card set.");
            if (!server.CurrentSession.CanVote) throw new VoteException($"Session not in state where players can vote.");
            if (!server.Players.ContainsKey(playerPrivateId)) throw new VoteException($"Player is not part of session.");

            var player = server.GetPlayer(playerPrivateId);
            if (player.Type == PlayerType.Observer) throw new VoteException($"Player is of type '{player.Type}' and cannot vote.");
            if (player.Mode == PlayerMode.Asleep) throw new VoteException($"Player is in mode '{player.Mode}', and cannot vote.");

            server.CurrentSession.SetVote(player.PublicId, vote);
            var autoShown = TryAutoShow(server);
            RaiseRoomUpdated(server.Id, server);
            RaiseLogUpdated(server.Id, player.Name, "Voted.");
            RaiseAutoShownLogIfApplicable(server, autoShown);
        }

        public void RedactVote(Guid serverId, string playerPrivateId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanVote) throw new VoteException($"Session not in state where players can unvote.");
            if (!server.Players.ContainsKey(playerPrivateId)) throw new VoteException($"Player is not part of session.");

            var player = server.GetPlayer(playerPrivateId);
            if (player.Type == PlayerType.Observer) throw new VoteException($"Player is of type '{player.Type}' and cannot vote.");
            if (player.Mode == PlayerMode.Asleep) throw new VoteException($"Player is in mode '{player.Mode}', and cannot vote.");

            server.CurrentSession.RemoveVote(player.PublicId);
            RaiseRoomUpdated(server.Id, server);
            RaiseLogUpdated(server.Id, player.Name, "Redacted their vote.");
        }

        public void ClearVotes(Guid serverId, string playerPrivateId)
        {
            var server = _serverStore.Get(serverId);
            var player = server.GetPlayer(playerPrivateId);
            EnsureCanControl(server, player);
            if (!server.CurrentSession.CanClear) throw new VoteException($"Session not in state where votes can be cleared.");

            server.CurrentSession.ClearVotes();
            RaiseRoomUpdated(server.Id, server);
            RaiseRoomCleared(server.Id);
            RaiseLogUpdated(server.Id, player.Name, "Cleared all votes.");
        }

        public void ShowVotes(Guid serverId, string playerPrivateId)
        {
            var server = _serverStore.Get(serverId);
            var player = server.GetPlayer(playerPrivateId);
            EnsureCanControl(server, player);
            if (!server.CurrentSession.CanShow(server.Players)) throw new VoteException($"Session not in state where votes can be shown.");

            server.CurrentSession.ShowVotes();
            _usageCounters.RoundPlayed();
            RaiseRoomUpdated(server.Id, server);
            RaiseLogUpdated(server.Id, player.Name, "Made all votes visible.");
        }

        public Player ChangePlayerType(Guid serverId, string playerPrivateId, PlayerType newType)
        {
            var server = _serverStore.Get(serverId);
            var player = server.GetPlayer(playerPrivateId);
            if (server.CurrentSession.HasVoted(player.PublicId))
            {
                throw new ChangePlayerTypeException($"Cannot change from playertype '{PlayerType.Participant}', when player has voted.");
            }

            server.ChangePlayerType(player, newType);
            var autoShown = TryAutoShow(server);
            RaiseRoomUpdated(server.Id, server);
            RaiseLogUpdated(server.Id, player.Name, $"Changed their player type to {newType}.");
            RaiseAutoShownLogIfApplicable(server, autoShown);
            return player;
        }

        public void SetAutoShow(Guid serverId, string playerPrivateId, bool enabled)
        {
            var server = _serverStore.Get(serverId);
            var player = server.GetPlayer(playerPrivateId);
            EnsureCanControl(server, player);

            server.CurrentSession.AutoShow = enabled;
            var autoShown = TryAutoShow(server);
            RaiseRoomUpdated(server.Id, server);
            RaiseLogUpdated(server.Id, player.Name, enabled ? "Enabled showing votes when everyone has voted." : "Disabled showing votes when everyone has voted.");
            RaiseAutoShownLogIfApplicable(server, autoShown);
        }

        private static bool TryAutoShow(PokerServer server)
        {
            var session = server.CurrentSession;
            if (!session.AutoShow || !session.CanShow(server.Players)) return false;

            session.ShowVotes();
            return true;
        }

        private void RaiseAutoShownLogIfApplicable(PokerServer server, bool autoShown)
        {
            if (autoShown)
            {
                RaiseLogUpdated(server.Id, "Room", "Everyone has voted. Votes are now visible.");
            }
        }

        public void SetAdmin(Guid serverId, string initiatingPlayerPrivateId, int playerPublicId, bool isAdmin)
        {
            var server = _serverStore.Get(serverId);
            var initiatingPlayer = server.GetPlayer(initiatingPlayerPrivateId);
            EnsureCanControl(server, initiatingPlayer);

            var targetPlayer = server.Players.Values.FirstOrDefault(p => p.PublicId == playerPublicId);
            if (targetPlayer == null) throw new PermissionException("Player is not part of session.");
            if (targetPlayer.IsAdmin == isAdmin) return;

            targetPlayer.IsAdmin = isAdmin;
            RaiseRoomUpdated(server.Id, server);
            RaiseLogUpdated(server.Id, initiatingPlayer.Name, isAdmin ? $"Made {targetPlayer.Name} admin." : $"Removed {targetPlayer.Name} as admin.");
        }

        private static void EnsureCanControl(PokerServer server, Player player)
        {
            if (!server.CanControl(player)) throw new PermissionException("Only admins can control this room.");
        }

        private IList<PokerServer> SetPlayerToSleepOnAllServers(IEnumerable<PokerServer> servers, string playerPrivateId)
        {
            var serversWithUser = servers.Where(s => s.Players.ContainsKey(playerPrivateId)).ToList();
            foreach (var server in serversWithUser)
            {
                server.SleepPlayer(playerPrivateId);
            }

            return serversWithUser;
        }

        private void RaiseRoomCleared(Guid serverId)
        {
            RoomCleared.Invoke(this, new RoomClearedEventArgs(serverId));
        }

        private void RaiseLogUpdated(Guid serverId, string initiatingPlayerName, string logMessage)
        {
            LogUpdated.Invoke(this, new LogUpdatedEventArgs(serverId, initiatingPlayerName, logMessage));
        }

        private void RaiseRoomUpdated(Guid serverId, PokerServer updatedServer)
        {
            RoomUpdated.Invoke(this, new RoomUpdatedEventArgs(serverId, updatedServer));
        }

        private void RaisePlayerKicked(Guid serverId, Player kickedPlayer)
        {
            PlayerKicked.Invoke(this, new PlayerKickedEventArgs(serverId, kickedPlayer));
        }
    }
}
