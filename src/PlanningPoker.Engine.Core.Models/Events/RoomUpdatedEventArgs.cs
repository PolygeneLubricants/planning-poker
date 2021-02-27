using System;

namespace PlanningPoker.Engine.Core.Models.Events
{
    public class RoomUpdatedEventArgs : PlanningPokerEventArgs
    {
        public RoomUpdatedEventArgs(
            Guid serverId,
            PokerServer updatedServer) : base(serverId)
        {
            UpdatedServer = updatedServer;
        }

        public PokerServer UpdatedServer { get; }
    }
}