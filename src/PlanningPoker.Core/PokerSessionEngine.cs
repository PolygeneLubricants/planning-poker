using System.Collections.Generic;
using PlanningPoker.Core.Models.Poker;

namespace PlanningPoker.Core
{
    public static class PokerSessionEngine
    {
        public static void SetVote(PokerSession session, string playerId, int vote)
        {
            session.Votes[playerId] = vote;
        }

        public static void RemoveVote(PokerSession session, string playerId)
        {
            session.Votes.Remove(playerId);
        }

        public static void Clear(PokerSession session)
        {
            session.Votes = new Dictionary<string, int>();
            session.IsShown = false;
        }

        public static void Show(PokerSession session)
        {
            session.IsShown = true;
        }

        public static void RemovePlayer(PokerSession session, string playerId)
        {
            session.Votes.Remove(playerId);
        }
    }
}