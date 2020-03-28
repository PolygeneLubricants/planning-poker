using System;
using System.Collections.Generic;
using PlanningPoker.Core.ViewModels.Poker;

namespace PlanningPoker.Core.ViewModels
{
    public class PokerServerViewModel
    {
        public Guid Id { get; set; }

        public IList<PlayerViewModel> Players { get; set; } = new List<PlayerViewModel>();

        public PokerSessionViewModel? CurrentSession { get; set; }
    }
}