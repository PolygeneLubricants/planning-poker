namespace PlanningPoker.Core.Models
{
    public class Player
    {
        public Player(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}