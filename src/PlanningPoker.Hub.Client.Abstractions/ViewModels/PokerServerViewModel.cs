using System;
using System.Collections.Generic;
using PlanningPoker.Hub.Client.Abstractions.ViewModels.Poker;

namespace PlanningPoker.Hub.Client.Abstractions.ViewModels
{
    public class PokerServerViewModel
    {
        public Guid Id { get; set; }

        public IList<PlayerViewModel>? Players { get; set; }

        public PokerSessionViewModel? CurrentSession { get; set; }
    }
}