using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_SleepPlayer : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_SleepPlayer(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Sleep_WhenServerDoesNotExist_Ok()
        {
            // Arrange
            var builder = CreateBuilder();

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.Disconnect());

            // Assert
            Assert.Null(exceptionRecord);
        }

        [Fact]
        public async Task Sleep_WhenServerExistAndPlayerDoesNotExist_Ok()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out _);

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.Disconnect());

            // Assert
            Assert.Null(exceptionRecord);
        }

        [Fact]
        public async Task Sleep_WhenPlayerHasVoted_PlayerKeepsVote()
        {
            // Arrange
            var expectedVote = "1";
            var disconnectingBuilder = CreateBuilder();
            disconnectingBuilder.WithServer(out var serverId);
            disconnectingBuilder.WithPlayer(serverId, out var disconnectingPlayer);
            disconnectingBuilder.WithPlayerVoted(serverId, disconnectingPlayer.Id, expectedVote);
            var observingBuilder = CreateBuilder()
                .WithPlayer(serverId, out _);
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            bool hasVoted = false;
            observingBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                hasVoted = viewModel.CurrentSession.Votes.ContainsKey(disconnectingPlayer.PublicId.ToString());
                awaitResponse.Release();
            });

            // Act
            await disconnectingBuilder.HubClient.Disconnect();

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.True(hasVoted);
        }

        [Fact]
        public async Task Sleep_WhenPlayerHasVoted_VotesCanBeShown()
        {
            // Arrange
            var expectedVote = "1";
            var disconnectingBuilder = CreateBuilder();
            disconnectingBuilder.WithServer(out var serverId);
            disconnectingBuilder.WithPlayer(serverId, out _);
            var observingBuilder = CreateBuilder()
                .WithPlayer(serverId, out var observingPlayer)
                .WithPlayerVoted(serverId, observingPlayer.Id, expectedVote);
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            bool canShow = false;
            observingBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                canShow = viewModel.CurrentSession.CanShow;
                awaitResponse.Release();
            });

            // Act
            await disconnectingBuilder.HubClient.Disconnect();

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.True(canShow);
        }

        [Fact]
        public async Task Sleep_WhenNoOneHasVoted_CannotClear()
        {
            // Arrange
            var expectedVote = "1";
            var disconnectingBuilder = CreateBuilder();
            disconnectingBuilder.WithServer(out var serverId);
            disconnectingBuilder.WithPlayer(serverId, out _);
            var observingBuilder = CreateBuilder()
                .WithPlayer(serverId, out _);
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            bool canClear = false;
            observingBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                canClear = viewModel.CurrentSession.CanClear;
                awaitResponse.Release();
            });

            // Act
            await disconnectingBuilder.HubClient.Disconnect();

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.False(canClear);
        }
    }
}
