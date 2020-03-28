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
            Players = new List<Player>();
            CurrentSession = new PokerSession();
        }

        public Guid Id { get; set; }

        public IList<Player> Players { get; set; }

        public PokerSession CurrentSession { get; set; }
    }
}