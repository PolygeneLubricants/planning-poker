using System;
using System.Threading;
using System.Threading.Tasks;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public partial class PlanningPokerHubTests
    {
        [Fact]
        public async Task OnPlayerKicked_WhenPlayerKicked_PlayerKickedSent()
        {
            // Arrange
            var player1Builder = CreateBuilder();
            player1Builder.WithServer(out var serverId);
            player1Builder.WithPlayer(serverId, out var player1);

            var player2Builder = CreateBuilder();
            player2Builder.WithPlayer(serverId, out var player2);

            var awaitKickResponse = new SemaphoreSlim(0);

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
            await awaitKickResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.True(kickCommandInvoked);
            Assert.Equal(player2.PublicId, kickedPlayer.PublicId);
            Assert.Null(kickedPlayer.Id);
        }
    }
}