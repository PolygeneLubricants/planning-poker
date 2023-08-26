using System;
using PlanningPoker.Engine.Core.Exceptions;
using PlanningPoker.Engine.Core.Managers;
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
    }

    public class PlanningPokerEngine : IPlanningPokerEngine
    {
        private readonly IServerStore _serverStore;
        private const int MaxPlayerNameLength = 20;

        public PlanningPokerEngine(
            IServerStore serverStore)
        {
            _serverStore = serverStore;
        }

        public event EventHandler<PlayerKickedEventArgs> PlayerKicked;
        public event EventHandler<RoomUpdatedEventArgs> RoomUpdated;
        public event EventHandler<LogUpdatedEventArgs> LogUpdated;
        public event EventHandler<RoomClearedEventArgs> RoomCleared;
        
        public void Kick(Guid id, string initiatingPlayerPrivateId, int playerPublicIdToRemove)
        {
            var server = _serverStore.Get(id);
            var player = ServerManager.GetPlayer(server, initiatingPlayerPrivateId);
            var wasRemoved = ServerManager.TryRemovePlayer(server, playerPublicIdToRemove, out var kickedPlayer);
            if (wasRemoved && kickedPlayer != null)
            {
                RaisePlayerKicked(id, kickedPlayer);
                RaiseRoomUpdated(id, server);
                RaiseLogUpdated(id, player.Name, $"Kicked {kickedPlayer.Name}.");
            }
        }

        public void SleepInAllRooms(string playerPrivateId)
        {
            var serversWithPlayer = ServerManager.SetPlayerToSleepOnAllServers(_serverStore.All(), playerPrivateId);
            foreach (var server in serversWithPlayer)
            {
                RaiseRoomUpdated(server.Id, server);
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
            var newPlayer = ServerManager.AddOrUpdatePlayer(server, recoveryId, playerPrivateId, formattedPlayerName, type);
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

            var player = ServerManager.GetPlayer(server, playerPrivateId);
            if (player.Type == PlayerType.Observer) throw new VoteException($"Player is of type '{player.Type}' and cannot vote.");
            if (player.Mode == PlayerMode.Asleep) throw new VoteException($"Player is in mode '{player.Mode}', and cannot vote.");

            SessionManager.SetVote(server.CurrentSession, player.PublicId, vote);
            RaiseRoomUpdated(server.Id, server);
            RaiseLogUpdated(server.Id, player.Name, "Voted.");
        }

        public void RedactVote(Guid serverId, string playerPrivateId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanVote) throw new VoteException($"Session not in state where players can unvote.");
            if (!server.Players.ContainsKey(playerPrivateId)) throw new VoteException($"Player is not part of session.");

            var player = ServerManager.GetPlayer(server, playerPrivateId);
            if (player.Type == PlayerType.Observer) throw new VoteException($"Player is of type '{player.Type}' and cannot vote.");
            if (player.Mode == PlayerMode.Asleep) throw new VoteException($"Player is in mode '{player.Mode}', and cannot vote.");

            SessionManager.RemoveVote(server.CurrentSession, player.PublicId);
            RaiseRoomUpdated(server.Id, server);
            RaiseLogUpdated(server.Id, player.Name, "Redacted their vote.");
        }

        public void ClearVotes(Guid serverId, string playerPrivateId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanClear) throw new VoteException($"Session not in state where votes can be cleared.");

            SessionManager.Clear(server.CurrentSession);
            var player = ServerManager.GetPlayer(server, playerPrivateId);
            RaiseRoomUpdated(server.Id, server);
            RaiseRoomCleared(server.Id);
            RaiseLogUpdated(server.Id, player.Name, "Cleared all votes.");
        }

        public void ShowVotes(Guid serverId, string playerPrivateId)
        {
            var server = _serverStore.Get(serverId);
            if (!server.CurrentSession.CanShow(server.Players)) throw new VoteException($"Session not in state where votes can be shown.");

            SessionManager.Show(server.CurrentSession);
            var player = ServerManager.GetPlayer(server, playerPrivateId); 
            RaiseRoomUpdated(server.Id, server);
            RaiseLogUpdated(server.Id, player.Name, "Made all votes visible.");
        }

        public Player ChangePlayerType(Guid serverId, string playerPrivateId, PlayerType newType)
        {
            var server = _serverStore.Get(serverId);
            var player = ServerManager.GetPlayer(server, playerPrivateId);
            if (SessionManager.HasVoted(server.CurrentSession, player.PublicId))
            {
                throw new ChangePlayerTypeException($"Cannot change from playertype '{PlayerType.Participant}', when player has voted.");
            }

            ServerManager.ChangePlayerType(server, player, newType);
            RaiseRoomUpdated(server.Id, server);
            RaiseLogUpdated(server.Id, player.Name, $"Changed their player type to {newType}.");
            return player;
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
