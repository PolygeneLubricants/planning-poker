using PlanningPoker.Core.Models;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;

namespace PlanningPoker.Server.ViewModelMappers
{
    public static class PlayerViewModelMapper
    {
        public static PlayerViewModel Map(this Player player, bool includePrivateId)
        {
            var viewModel = new PlayerViewModel
            {
                Id = includePrivateId ? player.Id : null,
                PublicId = player.PublicId,
                Name = player.Name,
                Type = player.Type.Map()
            };

            return viewModel;
        }
    }
}