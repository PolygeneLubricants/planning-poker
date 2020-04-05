using PlanningPoker.Core.Models.Poker;

namespace PlanningPoker.Core.Extensions
{
    public static class PokerSessionExtensions
    {
        public static void Vote(this PokerSession session, int playerPublicId, int vote)
        {
            PokerSessionEngine.SetVote(session, playerPublicId, vote);
        }

        public static void UnVote(this PokerSession session, int playerPublicId)
        {
            PokerSessionEngine.RemoveVote(session, playerPublicId);
        }

        public static void Clear(this PokerSession session)
        {
            PokerSessionEngine.Clear(session);
        }

        public static void Show(this PokerSession session)
        {
            PokerSessionEngine.Show(session);
        }

        public static void RemovePlayer(this PokerSession session, int playerPublicId)
        {
            PokerSessionEngine.RemovePlayer(session, playerPublicId);
        }
    }
}