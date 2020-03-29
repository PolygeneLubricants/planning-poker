using System;
using System.Collections.Generic;
using PlanningPoker.Shared.ViewModels.Poker;

namespace PlanningPoker.Shared.ViewModels
{
    public class PokerServerViewModel
    {
        public Guid Id { get; set; }

        public IList<PlayerViewModel>? Players { get; set; }

        public PokerSessionViewModel? CurrentSession { get; set; }
    }
}