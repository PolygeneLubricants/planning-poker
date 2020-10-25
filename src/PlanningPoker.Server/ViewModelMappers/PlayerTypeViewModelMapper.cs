using System;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;

namespace PlanningPoker.Server.ViewModelMappers
{
    public static class PlayerTypeViewModelMapper
    {
        public static PlayerType Map(this Shared.PlayerType playerType)
        {
            return playerType switch
            {
                Shared.PlayerType.Participant => PlayerType.Participant,
                Shared.PlayerType.Observer => PlayerType.Observer,
                _ => throw new ArgumentOutOfRangeException(nameof(playerType), playerType, null)
            };
        }
    }
}