﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public partial class PlanningPokerHubTests
    {
        [Fact]
        public async Task UnVote_WhenServerDoesNotExist_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();
            var validVote = "1";

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.UnVote(Guid.NewGuid(), Guid.NewGuid().ToString()));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task UnVote_WhenServerExistAndPlayerDoesNotExist_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            var validVote = "1";

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.UnVote(serverId, Guid.NewGuid().ToString()));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task UnVote_WhenPlayerHasNotVoted_PlayerVoteRetracted()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player);
            var voteExists = true;
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            builder.HubClient.OnSessionUpdated(viewModel =>
            {
                voteExists = viewModel.CurrentSession.Votes.ContainsKey(player.PublicId.ToString());
                awaitResponse.Release();
            });

            // Act
            await builder.HubClient.UnVote(serverId, player.Id);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.False(voteExists);
        }

        [Fact]
        public async Task UnVote_WhenPlayerHasVoted_PlayerVoteRetracted()
        {
            // Arrange
            var validVote = "1";
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player);
            builder.WithPlayerVoted(serverId, player.Id, validVote);
            var voteExists = true;
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            builder.HubClient.OnSessionUpdated(viewModel =>
            {
                voteExists = viewModel.CurrentSession.Votes.ContainsKey(player.PublicId.ToString());
                awaitResponse.Release();
            });

            // Act
            await builder.HubClient.UnVote(serverId, player.Id);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.False(voteExists);
        }
    }
}
