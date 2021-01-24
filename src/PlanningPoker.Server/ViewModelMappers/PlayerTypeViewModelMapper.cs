using System;
using PlayerType = PlanningPoker.Engine.Core.Models.PlayerType;

namespace PlanningPoker.Server.ViewModelMappers
{
    public static class PlayerTypeViewModelMapper
    {
        public static Hub.Client.Abstractions.ViewModels.PlayerType Map(this PlayerType playerType)
        {
            return playerType switch
            {
                PlayerType.Participant => Hub.Client.Abstractions.ViewModels.PlayerType.Participant,
                PlayerType.Observer => Hub.Client.Abstractions.ViewModels.PlayerType.Observer,
                _ => throw new ArgumentOutOfRangeException(nameof(playerType), playerType, null)
            };
        }
    }
}