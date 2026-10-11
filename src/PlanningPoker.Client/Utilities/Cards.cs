using System.Collections.Generic;
using System.Globalization;

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

        public static IReadOnlyDictionary<string, IList<string>> Presets => new Dictionary<string, IList<string>>
        {
            ["Fibonacci"] = DefaultCardSet,
            ["Modified Fibonacci"] = new List<string> { "0", "½", "1", "2", "3", "5", "8", "13", "20", "40", "100", "?" },
            ["T-shirt sizes"] = new List<string> { "XS", "S", "M", "L", "XL", "XXL", "?" },
            ["Powers of 2"] = new List<string> { "1", "2", "4", "8", "16", "32", "64" }
        };

        /// <summary>
        /// Mirrors the server-side validation: a card is a whole number, or 1-3 characters.
        /// </summary>
        public static bool IsValidCard(string card)
        {
            if (int.TryParse(card, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)) return true;
            return card.Length > 0 && card.Length < 4;
        }
    }
}
