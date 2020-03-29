using System;
using System.Collections.Generic;
using PlanningPoker.Core.Models.Poker;

namespace PlanningPoker.Core.Models
{
    public class PokerServer
    {
        public PokerServer(Guid id)
        {
            Id = id;
            Players = new Dictionary<string, Player>();
            CurrentSession = new PokerSession();
            Created = DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public IDictionary<string, Player> Players { get; set; }

        public PokerSession CurrentSession { get; set; }

        public DateTime Created { get; set; }
    }
}