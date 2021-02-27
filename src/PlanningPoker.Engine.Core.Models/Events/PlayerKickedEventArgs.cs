using System;

namespace PlanningPoker.Engine.Core.Models.Events
{
    public class PlayerKickedEventArgs : PlanningPokerEventArgs
    {
        public PlayerKickedEventArgs(
            Guid serverId,
            Player kickedPlayer) : base(serverId)
        {
            KickedPlayer = kickedPlayer;
        }

        public Player KickedPlayer { get; }
    }
}