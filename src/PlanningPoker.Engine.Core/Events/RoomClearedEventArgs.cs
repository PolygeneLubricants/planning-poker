using System;

namespace PlanningPoker.Engine.Core.Events
{
    public class RoomClearedEventArgs : PlanningPokerEventArgs
    {
        public RoomClearedEventArgs(
            Guid serverId) : base(serverId)
        {
        }
    }
}