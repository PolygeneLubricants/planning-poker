using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_SetSleep : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_SetSleep(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task SetSleep_WhenPlayerSleepsThemselves_PlayerIsAsleep()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out var player);
            var observedMode = PlayerMode.Awake;
            var awaitResponse = new SemaphoreSlim(0);
            playerBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                observedMode = viewModel.Players.Single(p => p.PublicId == player.PublicId).Mode;
                awaitResponse.Release();
            });

            // Act
            await playerBuilder.HubClient.SetSleep(serverId, player.PublicId, true);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.Equal(PlayerMode.Asleep, observedMode);
        }

        [Fact]
        public async Task SetSleep_WhenPlayerWakesThemselves_PlayerIsAwake()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out var player).WithPlayerAsleep(serverId, player.PublicId);
            var observedMode = PlayerMode.Asleep;
            var awaitResponse = new SemaphoreSlim(0);
            playerBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                observedMode = viewModel.Players.Single(p => p.PublicId == player.PublicId).Mode;
                awaitResponse.Release();
            });

            // Act
            await playerBuilder.HubClient.SetSleep(serverId, player.PublicId, false);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.Equal(PlayerMode.Awake, observedMode);
        }

        [Fact]
        public async Task SetSleep_WhenSleepingPlayerHasVoted_PlayerKeepsVote()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out var player).WithPlayerVoted(serverId, player.Id, "1");
            CreateBuilder().WithPlayer(serverId, out _);
            var hasVote = false;
            var awaitResponse = new SemaphoreSlim(0);
            playerBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                hasVote = viewModel.CurrentSession.Votes.ContainsKey(player.PublicId.ToString());
                awaitResponse.Release();
            });

            // Act
            await playerBuilder.HubClient.SetSleep(serverId, player.PublicId, true);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.True(hasVote);
        }

        [Fact]
        public async Task SetSleep_WhenOnlyMissingVoterSleeps_AutoShowRevealsVotes()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var voterBuilder = CreateBuilder().WithPlayer(serverId, out var voter);
            await voterBuilder.HubClient.SetAutoShow(serverId, true);
            voterBuilder.WithPlayerVoted(serverId, voter.Id, "1");
            var sleeperBuilder = CreateBuilder().WithPlayer(serverId, out var sleeper);
            var isShown = false;
            var awaitResponse = new SemaphoreSlim(0);
            voterBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                isShown = viewModel.CurrentSession.IsShown;
                awaitResponse.Release();
            });

            // Act
            await sleeperBuilder.HubClient.SetSleep(serverId, sleeper.PublicId, true);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.True(isShown);
        }

        [Fact]
        public async Task SetSleep_WhenSleepingPlayerVotes_PlayerWakesUp()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out var player).WithPlayerAsleep(serverId, player.PublicId);
            var observedMode = PlayerMode.Asleep;
            var awaitResponse = new SemaphoreSlim(0);
            playerBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                observedMode = viewModel.Players.Single(p => p.PublicId == player.PublicId).Mode;
                awaitResponse.Release();
            });

            // Act
            await playerBuilder.HubClient.Vote(serverId, player.Id, "1");

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.Equal(PlayerMode.Awake, observedMode);
        }

        [Fact]
        public void SetSleep_WhenSleepingPlayerRejoins_PlayerStaysAsleep()
        {
            // Arrange
            var recoveryId = Guid.NewGuid();
            CreateBuilder().WithServer(out var serverId);
            CreateBuilder().WithPlayer(serverId, recoveryId, out var player).WithPlayerAsleep(serverId, player.PublicId);

            // Act
            CreateBuilder().WithPlayer(serverId, recoveryId, out var rejoinedPlayer);

            // Assert
            Assert.Equal(player.PublicId, rejoinedPlayer.PublicId);
            Assert.Equal(PlayerMode.Asleep, rejoinedPlayer.Mode);
        }

        [Fact]
        public async Task SetSleep_WhenPlayerWithoutControlSleepsOther_ReturnsError()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            CreateBuilder().WithPlayer(serverId, out var admin).WithAdmin(serverId, admin.PublicId);
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out _);
            CreateBuilder().WithPlayer(serverId, out var other);

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => playerBuilder.HubClient.SetSleep(serverId, other.PublicId, true));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task SetSleep_WhenAdminSleepsOther_OtherIsAsleep()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var adminBuilder = CreateBuilder().WithPlayer(serverId, out var admin).WithAdmin(serverId, admin.PublicId);
            CreateBuilder().WithPlayer(serverId, out var other);
            var observedMode = PlayerMode.Awake;
            var awaitResponse = new SemaphoreSlim(0);
            adminBuilder.HubClient.OnSessionUpdated(viewModel =>
            {
                observedMode = viewModel.Players.Single(p => p.PublicId == other.PublicId).Mode;
                awaitResponse.Release();
            });

            // Act
            await adminBuilder.HubClient.SetSleep(serverId, other.PublicId, true);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.Equal(PlayerMode.Asleep, observedMode);
        }

        [Fact]
        public async Task SetSleep_WhenAdminWakesOther_ReturnsError()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var adminBuilder = CreateBuilder().WithPlayer(serverId, out var admin).WithAdmin(serverId, admin.PublicId);
            CreateBuilder().WithPlayer(serverId, out var other).WithPlayerAsleep(serverId, other.PublicId);

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => adminBuilder.HubClient.SetSleep(serverId, other.PublicId, false));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task SetSleep_WhenPlayerSleeps_LogMessageSent()
        {
            // Arrange
            CreateBuilder().WithServer(out var serverId);
            var playerBuilder = CreateBuilder().WithPlayer(serverId, out var player);
            LogMessage? logMessageReceived = null;
            var awaitResponse = new SemaphoreSlim(0);
            playerBuilder.HubClient.OnLogMessageReceived(logMessage =>
            {
                logMessageReceived = logMessage;
                awaitResponse.Release();
            });

            // Act
            await playerBuilder.HubClient.SetSleep(serverId, player.PublicId, true);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.NotNull(logMessageReceived);
            Assert.Equal(player.Name, logMessageReceived.User);
            Assert.Equal("Is taking a break.", logMessageReceived.Message);
        }
    }
}
