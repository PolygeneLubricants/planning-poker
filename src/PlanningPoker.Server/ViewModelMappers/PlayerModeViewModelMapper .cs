using System;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;

namespace PlanningPoker.Server.ViewModelMappers
{
    public static class PlayerModeViewModelMapper
    {
        public static PlayerMode Map(this Engine.Core.Models.PlayerMode playerMode)
        {
            return playerMode switch
            {
                Engine.Core.Models.PlayerMode.Awake => PlayerMode.Awake,
                Engine.Core.Models.PlayerMode.Asleep => PlayerMode.Asleep,
                _ => throw new ArgumentOutOfRangeException(nameof(playerMode), playerMode, null)
            };
        }
    }
}