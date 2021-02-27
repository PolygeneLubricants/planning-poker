using System.Linq;
using PlanningPoker.Engine.Core.Models;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;

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
                Players = server.Players.Select(p => p.Value.Map(includePrivateId: false)).ToList()
            };

            return viewModel;
        }
    }
}