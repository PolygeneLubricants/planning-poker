using System.Collections.Generic;
using PlanningPoker.Core.Models.Poker;

namespace PlanningPoker.Core
{
    public static class PokerSessionEngine
    {
        public static void SetVote(PokerSession session, int playerId, int vote)
        {
            session.Votes[playerId] = vote;
        }

        public static void Clear(PokerSession session)
        {
            session.Votes = new Dictionary<int, int>();
        }

        public static void Show(PokerSession session)
        {
            session.IsShown = true;
        }
    }
}