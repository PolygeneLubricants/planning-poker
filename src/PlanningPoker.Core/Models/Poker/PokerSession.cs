using System.Collections.Generic;
using System.Linq;

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

        public bool CanShow(IDictionary<string, Player> participants)
        {
            return 
                Votes.Count != 0 
                && Votes.Count == participants.Count(p => p.Value.Type == PlayerType.Participant) 
                && !IsShown;
        }

        public bool CanClear => Votes.Any();

        public bool CanVote => !IsShown;

        public IDictionary<int, int> Votes { get; set; }
    }
}