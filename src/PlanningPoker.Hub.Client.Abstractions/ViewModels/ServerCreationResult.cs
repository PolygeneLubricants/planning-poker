using System;

namespace PlanningPoker.Hub.Client.Abstractions.ViewModels
{
    public class ServerCreationResult
    {
        public bool Created { get; set; }

        public Guid? ServerId { get; set; }

        public string? ValidationMessage { get; set; }
    }
}