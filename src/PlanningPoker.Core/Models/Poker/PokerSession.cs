using System.Collections.Generic;

namespace PlanningPoker.Core.Models.Poker
{
    public class PokerSession
    {
        public PokerSession()
        {
            IsShown = false;
            Votes = new Dictionary<int, int>();
        }

        public bool IsShown { get; set; }

        public IDictionary<int, int> Votes { get; set; }
    }
}