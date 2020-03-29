using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Core.Models;
using PlanningPoker.Core.Models.Poker;
using PlanningPoker.Shared.ViewModels.Poker;

namespace PlanningPoker.Server.ViewModelMappers.Poker
{
    public static class PokerSessionViewModelMapper
    {
        public static PokerSessionViewModel Map(this PokerSession session, IDictionary<string, Player> participants)
        {
            var votes = session.IsShown 
                ? session.Votes.ToDictionary(pair => pair.Key, pair => pair.Value.ToString()) 
                : session.Votes.ToDictionary(pair => pair.Key, pair => "?");
            var viewModel = new PokerSessionViewModel
            {
                Votes = votes,
                IsShown = session.IsShown,
                CanClear = session.CanClear,
                CanShow = session.CanShow(participants),
                CanVote = session.CanVote
            };

            return viewModel;
        }
    }
}