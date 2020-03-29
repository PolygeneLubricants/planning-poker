using System.Linq;
using PlanningPoker.Core.Models;
using PlanningPoker.Server.ViewModelMappers.Poker;
using PlanningPoker.Shared.ViewModels;

namespace PlanningPoker.Server.ViewModelMappers
{
    public static class PokerServerViewModelMapper
    {
        public static PokerServerViewModel Map(this PokerServer server)
        {
            var viewModel = new PokerServerViewModel
            {
                Id = server.Id,
                CurrentSession = server.CurrentSession.Map(server.Players),
                Players = server.Players.Select(p => p.Value.Map()).ToList()
            };

            return viewModel;
        }
    }
}