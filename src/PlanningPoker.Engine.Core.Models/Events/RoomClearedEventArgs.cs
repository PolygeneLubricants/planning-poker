using System;

namespace PlanningPoker.Engine.Core.Models.Events
{
    public class RoomClearedEventArgs : PlanningPokerEventArgs
    {
        public RoomClearedEventArgs(
            Guid serverId) : base(serverId)
        {
        }
    }
}