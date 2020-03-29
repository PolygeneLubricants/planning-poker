using System.Collections.Generic;

namespace PlanningPoker.Core.ViewModels.Poker
{
    public class PokerSessionViewModel
    {
        public bool IsShown { get; set; }

        public IDictionary<string, int>? Votes { get; set; }
    }
}