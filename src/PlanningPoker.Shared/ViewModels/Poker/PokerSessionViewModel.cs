using System.Collections.Generic;

namespace PlanningPoker.Shared.ViewModels.Poker
{
    public class PokerSessionViewModel
    {
        public bool IsShown { get; set; }

        public IDictionary<string, int>? Votes { get; set; }
    }
}