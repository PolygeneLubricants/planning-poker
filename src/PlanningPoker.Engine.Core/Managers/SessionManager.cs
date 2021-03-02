using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Engine.Core.Models;
using PlanningPoker.Engine.Core.Models.Poker;

namespace PlanningPoker.Engine.Core.Managers
{
    internal static class SessionManager
    {
        internal static void SetVote(PokerSession session, int playerPublicId, string vote)
        {
            session.Votes[playerPublicId] = vote;
        }

        internal static void RemoveVote(PokerSession session, int playerPublicId)
        {
            session.Votes.Remove(playerPublicId);
        }

        internal static void Clear(PokerSession session)
        {
            session.Votes = new Dictionary<int, string>();
            session.IsShown = false;
        }

        internal static void Show(PokerSession session)
        {
            session.IsShown = true;
        }

        internal static void RemovePlayer(PokerSession session, int playerPublicId)
        {
            session.Votes.Remove(playerPublicId);
            if (!session.Votes.Any())
            {
                session.IsShown = false;
            }
        }

        public static bool HasVoted(PokerSession session, int playerPublicId)
        {
            return session.Votes.ContainsKey(playerPublicId);
        }
    }
}