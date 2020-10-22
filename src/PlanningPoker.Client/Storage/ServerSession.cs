using System;

namespace PlanningPoker.Client.Storage
{
    public class ServerSession
    {
        public Guid ServerId { get; set; }
        public string? Username { get; set; }
    }
}