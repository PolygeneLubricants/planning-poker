using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PlanningPoker.Hub.Client.Abstractions;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_Clear : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_Clear(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Clear_WhenServerDoesNotExist_ReturnsError()
        {
            // Arrange
            var serverBuilder = CreateBuilder();

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => serverBuilder.HubClient.ClearVotes(Guid.NewGuid()));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Clear_WhenThereAreNoVotes_ReturnsError()
        {
            // Arrange
            var serverBuilder = CreateBuilder();
            serverBuilder.WithServer(out var serverId);
            var playerConnections = new List<IPlanningPokerHubClient>
            {
                CreateBuilder().WithPlayer(serverId, out var player1).HubClient,
                CreateBuilder().WithPlayer(serverId, out var player2).HubClient,
                CreateBuilder().WithPlayer(serverId, out var player3).HubClient
            };

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => serverBuilder.HubClient.ClearVotes(serverId));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Clear_WhenThereAreSomeVotes_ThenNoVotes()
        {
            // Arrange
            var serverBuilder = CreateBuilder();
            serverBuilder.WithServer(out var serverId);
            var validVote = "1";
            var hasVotes = true;
            var playerConnections = new List<IPlanningPokerHubClient>
            {
                CreateBuilder().WithPlayer(serverId, out var player1).WithPlayerVoted(serverId, player1.Id, validVote).HubClient,
                CreateBuilder().WithPlayer(serverId, out var player2).WithPlayerVoted(serverId, player2.Id, validVote).HubClient,
                CreateBuilder().WithPlayer(serverId, out var player3).HubClient
            };
            
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            playerConnections[2].OnSessionUpdated(viewModel =>
            {
                hasVotes = viewModel.CurrentSession.Votes.Any();
                awaitResponse.Release();
            });

            // Act
            await playerConnections[2].ClearVotes(serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.False(hasVotes);
        }

        [Fact]
        public async Task Clear_WhenThereAreAllVotes_ThenNoVotes()
        {
            // Arrange
            var serverBuilder = CreateBuilder();
            serverBuilder.WithServer(out var serverId);
            var validVote = "1";
            var hasVotes = true;
            var playerConnections = new List<IPlanningPokerHubClient>
            {
                CreateBuilder().WithPlayer(serverId, out var player1).WithPlayerVoted(serverId, player1.Id, validVote).HubClient,
                CreateBuilder().WithPlayer(serverId, out var player2).WithPlayerVoted(serverId, player2.Id, validVote).HubClient,
                CreateBuilder().WithPlayer(serverId, out var player3).WithPlayerVoted(serverId, player3.Id, validVote).HubClient
            };
            
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            playerConnections[2].OnSessionUpdated(viewModel =>
            {
                hasVotes = viewModel.CurrentSession.Votes.Any();
                awaitResponse.Release();
            });

            // Act
            await playerConnections[2].ClearVotes(serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.False(hasVotes);
        }

        [Fact]
        public async Task Clear_WhenClearedByObserver_ThenNoVotes()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            builder.WithObserver(serverId, out var observer);
            var validVote = "1";
            var hasVotes = true;
            var playerConnections = new List<IPlanningPokerHubClient>
            {
                CreateBuilder().WithPlayer(serverId, out var player1).WithPlayerVoted(serverId, player1.Id, validVote).HubClient,
                CreateBuilder().WithPlayer(serverId, out var player2).HubClient,
                CreateBuilder().WithPlayer(serverId, out var player3).HubClient
            };
            
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            builder.HubClient.OnSessionUpdated(viewModel =>
            {
                hasVotes = viewModel.CurrentSession.Votes.Any();
                awaitResponse.Release();
            });

            // Act
            await builder.HubClient.ClearVotes(serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeoutProvider.GetDefaultTimeout());
            Assert.False(hasVotes);
        }
    }
}