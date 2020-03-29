using System.Collections.Generic;

namespace PlanningPoker.Shared.ViewModels.Poker
{
    public class PokerSessionViewModel
    {
        public bool IsShown { get; set; }

        public bool CanShow { get; set; }

        public bool CanClear { get; set; }

        public bool CanVote { get; set; }

        public IDictionary<string, string>? Votes { get; set; }
    }
}