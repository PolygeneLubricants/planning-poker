using PlanningPoker.Core.Models;
using PlanningPoker.Shared.ViewModels;

namespace PlanningPoker.Server.ViewModelMappers
{
    public static class PlayerViewModelMapper
    {
        public static PlayerViewModel Map(this Player player)
        {
            var viewModel = new PlayerViewModel
            {
                Id = player.Id,
                Name = player.Name
            };

            return viewModel;
        }
    }
}