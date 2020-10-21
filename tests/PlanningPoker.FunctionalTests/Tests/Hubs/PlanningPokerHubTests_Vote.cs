using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PlanningPoker.Core.Models;
using PlanningPoker.Shared;
using PlanningPoker.Shared.ViewModels;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public partial class PlanningPokerHubTests
    {
        [Fact]
        public async Task Vote_WhenServerDoesNotExist_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();
            var validVote = "1";

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.Connection.InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, Guid.NewGuid(), Guid.NewGuid(), validVote));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Vote_WhenServerExistAndPlayerDoesNotExist_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            var validVote = "1";

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.Connection.InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, Guid.NewGuid(), validVote));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Vote_WhenPlayerHasNotVoted_PlayerVotes()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player);
            var validVote = "1";
            string actualVote = null;
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            builder.Connection.On<PokerServerViewModel>(Messages.UPDATED, viewModel =>
            {
                actualVote = viewModel.CurrentSession.Votes[player.PublicId.ToString()];
                awaitResponse.Release();
            });

            // Act
            await builder.Connection.InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player.Id, validVote);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal("?", actualVote);
        }

        [Fact]
        public async Task Vote_WhenPlayerHasVoted_PlayerVotes()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player);
            var firstVote = "1";
            var secondVote = "21";
            string actualVote = null;
            await builder.Connection.InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player.Id, firstVote);
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            builder.Connection.On<PokerServerViewModel>(Messages.UPDATED, viewModel =>
            {
                actualVote = viewModel.CurrentSession.Votes[player.PublicId.ToString()];
                awaitResponse.Release();
            });

            // Act
            await builder.Connection.InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player.Id, secondVote);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal("?", actualVote);
        }

        [Theory]
        [MemberData(nameof(InvalidVotes))]
        public async Task Vote_WhenVoteDoesNotExistInCardSet_ReturnsError(string vote)
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player);

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.Connection.InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player.Id, vote));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Vote_WhenPlayerIsObserver_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            builder.WithObserver(serverId, out var observer);
            var validVote = "1";

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.Connection.InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, observer.Id, validVote));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        public static IEnumerable<object[]> InvalidVotes =>
            new List<object[]>
            {
                new object[] { "" },
                new object[] { "0" },
                new object[] { "22" },
                new object[] { (string)null },
                new object[] { "1,2" },
                new object[] { "1,2,3,5,8,13,21,?" }
            };
    }
}
