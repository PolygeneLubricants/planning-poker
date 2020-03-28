using PlanningPoker.Core.Models;

namespace PlanningPoker.Core.Extensions
{
    public static class PokerServerExtensions
    {
        public static Player Join(this PokerServer server, string playerName)
        {
            return PokerServerManager.AddPlayer(server, playerName);
        }
    }
}