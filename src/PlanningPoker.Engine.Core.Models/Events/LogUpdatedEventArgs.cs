using System;

namespace PlanningPoker.Engine.Core.Models.Events
{
    public class LogUpdatedEventArgs : PlanningPokerEventArgs
    {
        public LogUpdatedEventArgs(
            Guid serverId,
            string initiatingPlayer,
            string logMessage) : base(serverId)
        {
            InitiatingPlayer = initiatingPlayer;
            LogMessage = logMessage;
        }


        public string InitiatingPlayer { get; set; }

        public string LogMessage { get; set; }
    }
}