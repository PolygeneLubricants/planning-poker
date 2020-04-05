using System.Collections.Generic;
using PlanningPoker.Core.Models.Poker;

namespace PlanningPoker.Core
{
    public static class PokerSessionEngine
    {
        public static void SetVote(PokerSession session, int playerPublicId, int vote)
        {
            session.Votes[playerPublicId] = vote;
        }

        public static void RemoveVote(PokerSession session, int playerPublicId)
        {
            session.Votes.Remove(playerPublicId);
        }

        public static void Clear(PokerSession session)
        {
            session.Votes = new Dictionary<int, int>();
            session.IsShown = false;
        }

        public static void Show(PokerSession session)
        {
            session.IsShown = true;
        }

        public static void RemovePlayer(PokerSession session, int playerPublicId)
        {
            session.Votes.Remove(playerPublicId);
        }
    }
}