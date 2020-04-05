namespace PlanningPoker.Core.Models
{
    public class Player
    {
        public Player(
            string id, 
            int publicId, 
            string name)
        {
            Id = id;
            PublicId = publicId;
            Name = name;
        }
        
        public string Id { get; set; }
        
        public int PublicId { get; set; }

        public string Name { get; set; }
    }
}