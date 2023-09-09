using System;
using System.Threading;
using System.Threading.Tasks;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_OnLogMessageReceived : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_OnLogMessageReceived(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task OnLogMessageReceived_WhenVotesAreCleared_LogMessageSent()
        {
            // Arrange
            var builder = CreateBuilder();
            var validVote = "1";
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player);
            builder.WithPlayerVoted(serverId, player.Id, validVote);
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);

            LogMessage? logMessageReceived = null;
            builder.HubClient.OnLogMessageReceived(logMessage =>
            {
                logMessageReceived = logMessage;
                awaitResponse.Release();
            });

            // Act
            builder.WithVotesCleared(serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.NotNull(logMessageReceived);
            Assert.Equal(player.Name, logMessageReceived.User);
            Assert.NotNull(logMessageReceived.Message);
            Assert.NotEqual(default, logMessageReceived.Timestamp);
        }

        [Fact]
        public async Task OnLogMessageReceived_WhenPlayerJoins_LogMessageSent()
        {
            // Arrange
            var builder = CreateBuilder();
            var validVote = "1";
            builder.WithServer(out var serverId);
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);

            LogMessage? logMessageReceived = null;
            builder.HubClient.OnLogMessageReceived(logMessage =>
            {
                logMessageReceived = logMessage;
                awaitResponse.Release();
            });

            // Act
            builder.WithPlayer(serverId, out var player);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.NotNull(logMessageReceived);
            Assert.Equal(player.Name, logMessageReceived.User);
            Assert.NotNull(logMessageReceived.Message);
            Assert.NotEqual(default, logMessageReceived.Timestamp);
        }

        [Fact]
        public async Task OnLogMessageReceived_WhenPlayerKicked_LogMessageSent()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player1);
            var player2Builder = CreateBuilder();
            player2Builder.WithPlayer(serverId, out var player2);

            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);

            LogMessage? logMessageReceived = null;
            builder.HubClient.OnLogMessageReceived(logMessage =>
            {
                logMessageReceived = logMessage;
                awaitResponse.Release();
            });

            // Act
            builder.WithPlayerKicked(serverId, player1.Id, player2.PublicId);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.NotNull(logMessageReceived);
            Assert.Equal(player1.Name, logMessageReceived.User);
            Assert.NotNull(logMessageReceived.Message);
            Assert.NotEqual(default, logMessageReceived.Timestamp);
        }

        [Fact]
        public async Task OnLogMessageReceived_WhenVotesAreShown_LogMessageSent()
        {
            // Arrange
            var builder = CreateBuilder();
            var validVote = "1";
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player);
            builder.WithPlayerVoted(serverId, player.Id, validVote);

            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);

            LogMessage? logMessageReceived = null;
            builder.HubClient.OnLogMessageReceived(logMessage =>
            {
                logMessageReceived = logMessage;
                awaitResponse.Release();
            });

            // Act
            builder.WithVotesShown(serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.NotNull(logMessageReceived);
            Assert.Equal(player.Name, logMessageReceived.User);
            Assert.NotNull(logMessageReceived.Message);
            Assert.NotEqual(default, logMessageReceived.Timestamp);
        }

        [Fact]
        public async Task OnLogMessageReceived_WhenPlayerUnvotes_LogMessageSent()
        {
            // Arrange
            var builder = CreateBuilder();
            var validVote = "1";
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player);
            builder.WithPlayerVoted(serverId, player.Id, validVote);

            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);

            LogMessage? logMessageReceived = null;
            builder.HubClient.OnLogMessageReceived(logMessage =>
            {
                logMessageReceived = logMessage;
                awaitResponse.Release();
            });

            // Act
            builder.WithPlayerUnVoted(serverId, player.Id);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.NotNull(logMessageReceived);
            Assert.Equal(player.Name, logMessageReceived.User);
            Assert.NotNull(logMessageReceived.Message);
            Assert.NotEqual(default, logMessageReceived.Timestamp);
        }

        [Fact]
        public async Task OnLogMessageReceived_WhenPlayerVotes_LogMessageSent()
        {
            // Arrange
            var builder = CreateBuilder();
            var validVote = "1";
            builder.WithServer(out var serverId);
            builder.WithPlayer(serverId, out var player);

            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);

            LogMessage? logMessageReceived = null;
            builder.HubClient.OnLogMessageReceived(logMessage =>
            {
                logMessageReceived = logMessage;
                awaitResponse.Release();
            });

            // Act
            builder.WithPlayerVoted(serverId, player.Id, validVote);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.NotNull(logMessageReceived);
            Assert.Equal(player.Name, logMessageReceived.User);
            Assert.NotNull(logMessageReceived.Message);
            Assert.NotEqual(default, logMessageReceived.Timestamp);
        }
    }
}