using System.Collections.Generic;

namespace PlanningPoker.Client.Utilities
{
    public static class Cards
    {
        public static IList<string> DefaultCardSet => new List<string>
        {
            "1",
            "2",
            "3",
            "5",
            "8",
            "13",
            "21"
        };
    }
}