using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_ChangePlayer : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_ChangePlayer(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task ChangePlayerType_WhenServerDoesNotExist_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();
            var validType = PlayerType.Observer;

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.ChangePlayerType(Guid.NewGuid(), validType));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task ChangePlayerType_WhenServerExistAndPlayerDoesNotExist_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            var validType = PlayerType.Observer;

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.ChangePlayerType(serverId, validType));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task ChangePlayerType_WhenPlayerHasNotVoted_PlayerTypeChanged()
        {
            // Arrange
            var desiredNewPlayerType = PlayerType.Observer;
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player);
            PlayerViewModel? updatedPlayer = null;
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            builder.HubClient.OnSessionUpdated(viewModel =>
            {
                updatedPlayer = viewModel.Players.Single(p => p.PublicId == player.PublicId);
                awaitResponse.Release();
            });

            // Act
            await builder.HubClient.ChangePlayerType(serverId, desiredNewPlayerType);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.NotNull(updatedPlayer);
            Assert.Equal(desiredNewPlayerType, updatedPlayer.Type);
        }

        [Fact]
        public async Task ChangePlayerType_WhenPlayerHasVoted_Throws()
        {
            // Arrange
            var desiredNewPlayerType = PlayerType.Observer;
            var validVote = "1";
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player);
            builder.WithPlayerVoted(serverId, player.Id, validVote);

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.ChangePlayerType(serverId, desiredNewPlayerType));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task ChangePlayerType_WhenPlayerIsObserver_PlayerTypeChanged()
        {
            // Arrange
            var desiredNewPlayerType = PlayerType.Participant;
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            builder.WithObserver(serverId, out var player);
            PlayerViewModel? updatedPlayer = null;
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            builder.HubClient.OnSessionUpdated(viewModel =>
            {
                updatedPlayer = viewModel.Players.Single(p => p.PublicId == player.PublicId);
                awaitResponse.Release();
            });

            // Act
            await builder.HubClient.ChangePlayerType(serverId, desiredNewPlayerType);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.NotNull(updatedPlayer);
            Assert.Equal(desiredNewPlayerType, updatedPlayer.Type);
        }
    }
}
