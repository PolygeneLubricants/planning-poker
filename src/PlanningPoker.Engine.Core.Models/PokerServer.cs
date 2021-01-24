using System;
using System.Collections.Generic;
using PlanningPoker.Engine.Core.Models.Poker;

namespace PlanningPoker.Engine.Core.Models
{
    public class PokerServer
    {
        public PokerServer(Guid id, IList<string> cardSet)
        {
            Id = id;
            Players = new Dictionary<string, Player>();
            CurrentSession = new PokerSession(cardSet);
            Created = DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public IDictionary<string, Player> Players { get; set; }

        public PokerSession CurrentSession { get; set; }

        public DateTime Created { get; set; }
    }
}