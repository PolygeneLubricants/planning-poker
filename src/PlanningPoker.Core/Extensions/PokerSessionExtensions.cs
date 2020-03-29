using PlanningPoker.Core.Models.Poker;

namespace PlanningPoker.Core.Extensions
{
    public static class PokerSessionExtensions
    {
        public static void Vote(this PokerSession session, string playerId, int vote)
        {
            PokerSessionEngine.SetVote(session, playerId, vote);
        }

        public static void Clear(this PokerSession session)
        {
            PokerSessionEngine.Clear(session);
        }

        public static void Show(this PokerSession session)
        {
            PokerSessionEngine.Show(session);
        }

        public static void RemovePlayer(this PokerSession session, string playerId)
        {
            PokerSessionEngine.RemovePlayer(session, playerId);
        }
    }
}