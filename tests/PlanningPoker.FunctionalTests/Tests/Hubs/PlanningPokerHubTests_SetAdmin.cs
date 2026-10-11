using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_SetAdmin : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_SetAdmin(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task SetAdmin_WhenRoomHasNoAdmins_PlayerBecomesAdmin()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out var player);

            var isAdmin = false;
            var awaitResponse = new SemaphoreSlim(0);
            playerBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                isAdmin = viewModel.Players.Single(p => p.PublicId == player.PublicId).IsAdmin;
                awaitResponse.Release();
            });

            // Act
            await playerBuilder.HubClient.SetAdmin(serverId, player.PublicId, true);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.True(isAdmin);
        }

        [Fact]
        public async Task SetAdmin_WhenNonAdminTriesToBecomeAdmin_ReturnsError()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var adminBuilder = CreateBuilder().WithPlayer(serverId, out var admin).WithAdmin(serverId, admin.PublicId);
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out var player);

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => playerBuilder.HubClient.SetAdmin(serverId, player.PublicId, true));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Show_WhenRoomHasAdminAndPlayerIsNotAdmin_ReturnsError()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            CreateBuilder().WithPlayer(serverId, out var admin).WithAdmin(serverId, admin.PublicId).WithPlayerVoted(serverId, admin.Id, "1");
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out var player).WithPlayerVoted(serverId, player.Id, "1");

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => playerBuilder.HubClient.ShowVotes(serverId));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Show_WhenRoomHasAdminAndPlayerIsAdmin_VotesAreShown()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var adminBuilder = CreateBuilder().WithPlayer(serverId, out var admin).WithAdmin(serverId, admin.PublicId).WithPlayerVoted(serverId, admin.Id, "1");
            CreateBuilder().WithPlayer(serverId, out var player).WithPlayerVoted(serverId, player.Id, "1");

            var isShown = false;
            var awaitResponse = new SemaphoreSlim(0);
            adminBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                isShown = viewModel.CurrentSession.IsShown;
                awaitResponse.Release();
            });

            // Act
            await adminBuilder.HubClient.ShowVotes(serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.True(isShown);
        }

        [Fact]
        public async Task Kick_WhenRoomHasAdminAndPlayerIsNotAdmin_ReturnsError()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            CreateBuilder().WithPlayer(serverId, out var admin).WithAdmin(serverId, admin.PublicId);
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out var player);

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => playerBuilder.HubClient.KickPlayer(serverId, player.Id, admin.PublicId));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Show_WhenOnlyAdminIsAsleep_AnyoneCanShow()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var adminBuilder = CreateBuilder().WithPlayer(serverId, out var admin).WithAdmin(serverId, admin.PublicId);
            await adminBuilder.HubClient.Disconnect();
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out var player).WithPlayerVoted(serverId, player.Id, "1");

            var isShown = false;
            var awaitResponse = new SemaphoreSlim(0);
            playerBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                isShown = viewModel.CurrentSession.IsShown;
                awaitResponse.Release();
            });

            // Act
            await playerBuilder.HubClient.ShowVotes(serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.True(isShown);
        }
    }
}
