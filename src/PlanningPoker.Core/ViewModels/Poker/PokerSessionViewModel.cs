using System.Collections.Generic;

namespace PlanningPoker.Core.ViewModels.Poker
{
    public class PokerSessionViewModel
    {
        public bool IsShown { get; set; }

        public IDictionary<int, int> Votes { get; set; } = new Dictionary<int, int>();
    }
}