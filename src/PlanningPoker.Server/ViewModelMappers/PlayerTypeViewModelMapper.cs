using System;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;

namespace PlanningPoker.Server.ViewModelMappers
{
    public static class PlayerTypeViewModelMapper
    {
        public static PlayerType Map(this Engine.Core.Models.PlayerType playerType)
        {
            return playerType switch
            {
                Engine.Core.Models.PlayerType.Participant => PlayerType.Participant,
                Engine.Core.Models.PlayerType.Observer => PlayerType.Observer,
                _ => throw new ArgumentOutOfRangeException(nameof(playerType), playerType, null)
            };
        }
    }
}