using System;

namespace PlanningPoker.Shared.ViewModels
{
    public class ServerCreationResult
    {
        public bool Created { get; set; }

        public Guid? ServerId { get; set; }

        public string? ValidationMessage { get; set; }
    }
}