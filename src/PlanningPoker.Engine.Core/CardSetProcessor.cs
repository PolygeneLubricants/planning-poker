using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PlanningPoker.Engine.Core
{
    internal static class CardSetProcessor
    {
        internal static bool TryParseCardSet(string rawCardSet, out IList<string>? cardSet, out string? validationMessage)
        {
            var splitCards = rawCardSet?.Split(',')?.Select(c => c.Trim())?.ToList();
            if (splitCards == null || !splitCards.Any())
            {
                validationMessage = "Desired card set is empty. Please specify a valid card set.";
                cardSet = null;
                return false;
            }

            if (!splitCards.All(IsValidCard))
            {
                validationMessage = "One or more cards are invalid. Please ensure cards are either whole numbers, or single characters.";
                cardSet = null;
                return false;
            }

            var distinctCards = splitCards.Distinct().ToList();

            validationMessage = null;
            cardSet = distinctCards;
            return true;
        }

        internal static bool IsValidCard(string card)
        {
            var isNumber = int.TryParse(card, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedCard);
            if (!isNumber) return card.Length == 1;

            return true;
        }
    }
}