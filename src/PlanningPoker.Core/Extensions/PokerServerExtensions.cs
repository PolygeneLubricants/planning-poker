using PlanningPoker.Core.Models;

namespace PlanningPoker.Core.Extensions
{
    public static class PokerServerExtensions
    {
        public static Player Join(this PokerServer server, string playerId, string playerName, PlayerType type)
        {
            return PokerServerManager.AddPlayer(server, playerId, playerName, type);
        }

        public static void Remove(this PokerServer server, string playerId)
        {
            PokerServerManager.RemovePlayer(server, playerId);
        }

        public static Player GetPlayer(this PokerServer server, string playerId)
        {
            return PokerServerManager.GetPlayer(server, playerId);
        }
    }
}