using PlanningPoker.Core.Models;

namespace PlanningPoker.Shared.ViewModels
{
    public class PlayerViewModel
    {
        public string? Id { get; set; }

        public int PublicId { get; set; }

        public string? Name { get; set; }

        public PlayerType Type { get; set; }
    }
}