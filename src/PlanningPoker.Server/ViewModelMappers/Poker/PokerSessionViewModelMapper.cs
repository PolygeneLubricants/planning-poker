using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Core.Models;
using PlanningPoker.Core.Models.Poker;
using PlanningPoker.Shared.ViewModels.Poker;

namespace PlanningPoker.Server.ViewModelMappers.Poker
{
    public static class PokerSessionViewModelMapper
    {
        public static PokerSessionViewModel Map(this PokerSession session, IList<Player> participants)
        {
            var viewModel = new PokerSessionViewModel
            {
                Votes = session.Votes.ToDictionary(pair => pair.Key.ToString(), pair => pair.Value),
                IsShown = session.IsShown,
                CanClear = session.CanClear,
                CanShow = session.CanShow(participants),
                CanVote = session.CanVote
            };

            return viewModel;
        }
    }
}