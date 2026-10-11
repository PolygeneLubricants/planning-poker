using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_AutoShow : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_AutoShow(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task AutoShow_WhenEnabledAndEveryoneVotes_VotesAreShown()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var player1Builder = CreateBuilder().WithPlayer(serverId, out var player1);
            var player2Builder = CreateBuilder().WithPlayer(serverId, out var player2);
            await player1Builder.HubClient.SetAutoShow(serverId, true);
            player1Builder.WithPlayerVoted(serverId, player1.Id, "1");

            var isShown = false;
            var awaitResponse = new SemaphoreSlim(0);
            player1Builder.HubClient.OnSessionUpdated(viewModel =>
            {
                isShown = viewModel.CurrentSession.IsShown;
                awaitResponse.Release();
            });

            // Act
            await player2Builder.HubClient.Vote(serverId, player2.Id, "2");

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.True(isShown);
        }

        [Fact]
        public async Task AutoShow_WhenDisabledAndEveryoneVotes_VotesAreHidden()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var player1Builder = CreateBuilder().WithPlayer(serverId, out var player1).WithPlayerVoted(serverId, player1.Id, "1");
            var player2Builder = CreateBuilder().WithPlayer(serverId, out var player2);

            var isShown = true;
            var awaitResponse = new SemaphoreSlim(0);
            player1Builder.HubClient.OnSessionUpdated(viewModel =>
            {
                isShown = viewModel.CurrentSession.IsShown;
                awaitResponse.Release();
            });

            // Act
            await player2Builder.HubClient.Vote(serverId, player2.Id, "2");

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.False(isShown);
        }

        [Fact]
        public async Task AutoShow_WhenEnabledAfterEveryoneVoted_VotesAreShown()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out var player).WithPlayerVoted(serverId, player.Id, "1");

            var isShown = false;
            var autoShow = false;
            var awaitResponse = new SemaphoreSlim(0);
            playerBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                isShown = viewModel.CurrentSession.IsShown;
                autoShow = viewModel.CurrentSession.AutoShow;
                awaitResponse.Release();
            });

            // Act
            await playerBuilder.HubClient.SetAutoShow(serverId, true);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.True(autoShow);
            Assert.True(isShown);
        }
    }
}
