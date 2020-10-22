using System;
using System.Threading;
using System.Threading.Tasks;
using PlanningPoker.Shared.ViewModels;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public partial class PlanningPokerHubTests
    {
        [Fact]
        public async Task Kick_WhenServerIsMissing_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.KickPlayer(Guid.NewGuid(), Guid.NewGuid().ToString(), 1));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Kick_WhenServerExistsAndPlayerDoesNotExist_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.KickPlayer(serverId, Guid.NewGuid().ToString(), 1));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Kick_WhenServerExistsAndPlayerExists_PlayerIsKicked()
        {
            // Arrange
            var player1Builder = CreateBuilder();
            player1Builder.WithServer(out var serverId);
            player1Builder.WithPlayer(serverId, out var player1);

            var player2Builder = CreateBuilder();
            player2Builder.WithPlayer(serverId, out var player2);

            var awaitUpdateResponse = new SemaphoreSlim(0);
            var awaitKickResponse = new SemaphoreSlim(0);
            var playerCount = 2;
            player1Builder.HubClient.OnSessionUpdated(viewModel =>
            {
                playerCount = viewModel.Players.Count;
                awaitUpdateResponse.Release();
            });

            var kickCommandInvoked = false;
            PlayerViewModel kickedPlayer = null;
            player1Builder.HubClient.OnPlayerKicked(viewModel =>
            {
                kickCommandInvoked = true;
                kickedPlayer = viewModel;
                awaitKickResponse.Release();
            });

            // Act
            await player1Builder.HubClient.KickPlayer(serverId, player1.Id, player2.PublicId);

            // Assert
            await awaitUpdateResponse.WaitAsync(TimeSpan.FromSeconds(5));
            await awaitKickResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(1, playerCount);
            Assert.True(kickCommandInvoked);
            Assert.Equal(player2.PublicId, kickedPlayer.PublicId);
            Assert.Null(kickedPlayer.Id);
        }
    }
}
