﻿using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Engine.Core.Models;
using PlanningPoker.Engine.Core.Models.Poker;
using PlanningPoker.Hub.Client.Abstractions.ViewModels.Poker;

namespace PlanningPoker.Server.ViewModelMappers
{
    public static class PokerSessionViewModelMapper
    {
        public static PokerSessionViewModel Map(this PokerSession session, IDictionary<string, Player> participants)
        {
            var votes = MapVotes(session);
            var viewModel = new PokerSessionViewModel
            {
                Votes = votes,
                IsShown = session.IsShown,
                CanClear = session.CanClear,
                CanShow = session.CanShow(participants),
                CanVote = session.CanVote,
                CardSet = session.CardSet
            };

            return viewModel;
        }

        private static IDictionary<string, string> MapVotes(PokerSession session)
        {
            var votes = session.IsShown
                ? session.Votes.ToDictionary(pair => pair.Key.ToString(), pair => pair.Value.ToString())
                : session.Votes.ToDictionary(pair => pair.Key.ToString(), pair => "?");

            return votes;
        }
    }
}