using System.Collections.Generic;

namespace PlanningPoker.Shared
{
    public static class Cards
    {
        public static IList<int> Values => new List<int>
        {
            1,
            2,
            3,
            5,
            8,
            13,
            21
        };
    }
}