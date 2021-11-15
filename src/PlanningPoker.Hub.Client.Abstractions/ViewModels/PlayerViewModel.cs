namespace PlanningPoker.Hub.Client.Abstractions.ViewModels
{
    public class PlayerViewModel
    {
        public string? Id { get; set; }

        public int PublicId { get; set; }

        public string? Name { get; set; }

        public PlayerType Type { get; set; }

        public PlayerMode Mode { get; set; }
    }
}