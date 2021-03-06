﻿namespace PlanningPoker.Engine.Core.Models
{
    public class Player
    {
        public Player(
            string id, 
            int publicId, 
            string name, 
            PlayerType type)
        {
            Id = id;
            PublicId = publicId;
            Name = name;
            Type = type;
        }
        
        public string Id { get; set; }
        
        public int PublicId { get; set; }

        public string Name { get; set; }

        public PlayerType Type { get; set; }
    }
}