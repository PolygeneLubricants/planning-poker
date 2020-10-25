using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Shared;

namespace PlanningPoker.Core.Models.Poker
{
    public class PokerSession
    {
        public PokerSession(IList<string> cardSet)
        {
            IsShown = false;
            Votes = new Dictionary<int, string>();
            CardSet = cardSet;
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

        public IDictionary<int, string> Votes { get; set; }

        public IList<string> CardSet { get; set; }
    }
}