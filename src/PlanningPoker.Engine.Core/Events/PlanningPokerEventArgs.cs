using System;

namespace PlanningPoker.Engine.Core.Events
{
    public abstract class PlanningPokerEventArgs
    {
        protected PlanningPokerEventArgs(Guid serverId)
        {
            ServerId = serverId;
        }

        public Guid ServerId { get; }
    }
}