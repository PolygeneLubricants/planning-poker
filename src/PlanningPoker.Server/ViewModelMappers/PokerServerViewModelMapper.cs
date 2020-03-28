using System.Linq;
using PlanningPoker.Core.Models;
using PlanningPoker.Core.ViewModels;
using PlanningPoker.Server.ViewModelMappers.Poker;

namespace PlanningPoker.Server.ViewModelMappers
{
    public static class PokerServerViewModelMapper
    {
        public static PokerServerViewModel Map(this PokerServer server)
        {
            var viewModel = new PokerServerViewModel
            {
                Id = server.Id,
                CurrentSession = server.CurrentSession.Map(),
                Players = server.Players.Select(p => p.Map()).ToList()
            };

            return viewModel;
        }
    }
}