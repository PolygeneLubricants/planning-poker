using System;
using PlanningPoker.Engine.Core.Models;

namespace PlanningPoker.Engine.Core.Events
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