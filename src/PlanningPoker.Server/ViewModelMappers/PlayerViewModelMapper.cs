using PlanningPoker.Core.Models;
using PlanningPoker.Shared.ViewModels;

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
                Name = player.Name
            };

            return viewModel;
        }
    }
}