using System;
using PlanningPoker.Engine.Core.Models;

namespace PlanningPoker.Engine.Core.Events
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