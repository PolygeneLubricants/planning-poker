using System;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;

namespace PlanningPoker.Client.Storage
{
    public class ServerSession
    {
        public Guid ServerId { get; set; }
        public string? Username { get; set; }
        public PlayerType Type { get; set; }
    }
}