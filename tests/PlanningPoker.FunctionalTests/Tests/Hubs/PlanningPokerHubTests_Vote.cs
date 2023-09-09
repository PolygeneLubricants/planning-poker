using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_Vote : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_Vote(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Vote_WhenServerDoesNotExist_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();
            var validVote = "1";

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.Vote(Guid.NewGuid(), Guid.NewGuid().ToString(), validVote));

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
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.Vote(serverId, Guid.NewGuid().ToString(), validVote));

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
            builder.HubClient.OnSessionUpdated(viewModel =>
            {
                actualVote = viewModel.CurrentSession.Votes[player.PublicId.ToString()];
                awaitResponse.Release();
            });

            // Act
            await builder.HubClient.Vote(serverId, player.Id, validVote);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
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
            builder.WithPlayerVoted(serverId, player.Id, firstVote);
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            builder.HubClient.OnSessionUpdated(viewModel =>
            {
                actualVote = viewModel.CurrentSession.Votes[player.PublicId.ToString()];
                awaitResponse.Release();
            });

            // Act
            await builder.HubClient.Vote(serverId, player.Id, secondVote);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
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
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.Vote(serverId, player.Id, vote));

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
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.Vote(serverId, observer.Id, validVote));

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
