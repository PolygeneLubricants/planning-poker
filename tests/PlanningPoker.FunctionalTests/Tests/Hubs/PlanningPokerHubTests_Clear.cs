using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PlanningPoker.Core.Models;
using PlanningPoker.Shared;
using PlanningPoker.Shared.ViewModels;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public partial class PlanningPokerHubTests
    {
        [Fact]
        public async Task Clear_WhenServerDoesNotExist_ReturnsError()
        {
            // Arrange
            var serverBuilder = CreateBuilder();

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => serverBuilder.Connection.InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Clear, Guid.NewGuid()));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Clear_WhenThereAreNoVotes_ReturnsError()
        {
            // Arrange
            var serverBuilder = CreateBuilder();
            serverBuilder.WithServer(out var serverId);
            var playerConnections = new List<HubConnection>
            {
                CreateBuilder().WithPlayer(serverId, out var player1).Connection,
                CreateBuilder().WithPlayer(serverId, out var player2).Connection,
                CreateBuilder().WithPlayer(serverId, out var player3).Connection
            };

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => serverBuilder.Connection.InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Clear, serverId));

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
            var playerConnections = new List<HubConnection>
            {
                CreateBuilder().WithPlayer(serverId, out var player1).Connection,
                CreateBuilder().WithPlayer(serverId, out var player2).Connection,
                CreateBuilder().WithPlayer(serverId, out var player3).Connection
            };

            await playerConnections[0].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player1.Id, validVote);
            await playerConnections[1].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player2.Id, validVote);

            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            playerConnections[2].On<PokerServerViewModel>(Messages.UPDATED, viewModel =>
            {
                hasVotes = viewModel.CurrentSession.Votes.Any();
                awaitResponse.Release();
            });

            // Act
            await playerConnections[2].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Clear, serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
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
            var playerConnections = new List<HubConnection>
            {
                CreateBuilder().WithPlayer(serverId, out var player1).Connection,
                CreateBuilder().WithPlayer(serverId, out var player2).Connection,
                CreateBuilder().WithPlayer(serverId, out var player3).Connection
            };

            await playerConnections[0].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player1.Id, validVote);
            await playerConnections[1].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player2.Id, validVote);
            await playerConnections[2].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player3.Id, validVote);

            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            playerConnections[2].On<PokerServerViewModel>(Messages.UPDATED, viewModel =>
            {
                hasVotes = viewModel.CurrentSession.Votes.Any();
                awaitResponse.Release();
            });

            // Act
            await playerConnections[2].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Clear, serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
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
            var playerConnections = new List<HubConnection>
            {
                CreateBuilder().WithPlayer(serverId, out var player1).Connection,
                CreateBuilder().WithPlayer(serverId, out var player2).Connection,
                CreateBuilder().WithPlayer(serverId, out var player3).Connection
            };

            await playerConnections[0].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player1.Id, validVote);

            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            builder.Connection.On<PokerServerViewModel>(Messages.UPDATED, viewModel =>
            {
                hasVotes = viewModel.CurrentSession.Votes.Any();
                awaitResponse.Release();
            });

            // Act
            await builder.Connection.InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Clear, serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.False(hasVotes);
        }
    }
}