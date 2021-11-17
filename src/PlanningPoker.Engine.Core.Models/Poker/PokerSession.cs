using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PlanningPoker.Engine.Core.Models.Poker
{
    public class PokerSession
    {
        public PokerSession(IList<string> cardSet)
        {
            IsShown = false;
            Votes = new ConcurrentDictionary<int, string>();
            CardSet = cardSet;
        }

        public bool IsShown { get; set; }

        public bool CanShow(IDictionary<string, Player> participants)
        {
            return 
                Votes.Count != 0
                && !IsShown
                && AllAwakeParticipantsVoted(participants);
        }

        private bool AllAwakeParticipantsVoted(IDictionary<string, Player> participants)
        {
            var awakeParticipants = participants.Values.Where(p => p.Type == PlayerType.Participant && p.Mode == PlayerMode.Awake).Select(p => p.PublicId);
            return awakeParticipants.All(id => Votes.ContainsKey(id));
        }

        public bool CanClear => Votes.Any();

        public bool CanVote => !IsShown;

        public IDictionary<int, string> Votes { get; set; }

        public IList<string> CardSet { get; set; }
    }
}