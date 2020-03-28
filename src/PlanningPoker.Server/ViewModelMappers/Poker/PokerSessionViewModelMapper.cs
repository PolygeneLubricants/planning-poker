using PlanningPoker.Core.Models.Poker;
using PlanningPoker.Core.ViewModels.Poker;

namespace PlanningPoker.Server.ViewModelMappers.Poker
{
    public static class PokerSessionViewModelMapper
    {
        public static PokerSessionViewModel Map(this PokerSession session)
        {
            var viewModel = new PokerSessionViewModel
            {
                Votes = session.Votes,
                IsShown = session.IsShown
            };

            return viewModel;
        }
    }
}