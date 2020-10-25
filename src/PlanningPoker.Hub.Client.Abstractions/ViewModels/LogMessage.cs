using System;

namespace PlanningPoker.Hub.Client.Abstractions.ViewModels
{
    public class LogMessage
    {
        public string User { get; set; }

        public string Message { get; set; }

        public DateTime Timestamp { get; set; }
    }
}