using PlanningPoker.Core.Models;

namespace PlanningPoker.Core.Extensions
{
    public static class PokerServerExtensions
    {
        public static Player Join(this PokerServer server, string playerId, string playerName)
        {
            return PokerServerManager.AddPlayer(server, playerId, playerName);
        }

        public static void Remove(this PokerServer server, string playerId)
        {
            PokerServerManager.RemovePlayer(server, playerId);
        }
    }
}