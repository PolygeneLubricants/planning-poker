using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PlanningPoker.Shared;
using PlanningPoker.Shared.ViewModels;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public partial class PlanningPokerHubTests
    {
        [Fact]
        public async Task Show_WhenServerDoesNotExist_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.Connection.InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Show, Guid.NewGuid()));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Show_WhenNotEveryoneHasVoted_ReturnsError()
        {
            // Arrange
            var serverBuilder = CreateBuilder();
            serverBuilder.WithServer(out var serverId);
            var validVote = "1";
            var playerConnections = new List<HubConnection>
            {
                CreateBuilder().WithPlayer(serverId, out var player1).Connection,
                CreateBuilder().WithPlayer(serverId, out var player2).Connection,
                CreateBuilder().WithPlayer(serverId, out var player3).Connection
            };

            await playerConnections[0].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player1.Id, validVote);
            await playerConnections[1].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player2.Id, validVote);
            
            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => playerConnections[2].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Show, serverId));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Show_WhenEveryoneHasVoted_VotesAreShown()
        {
            // Arrange
            var serverBuilder = CreateBuilder();
            serverBuilder.WithServer(out var serverId);
            var validVote = "1";
            var isShown = false;
            var actualVotes = new List<string>();
            var playerConnections = new List<HubConnection>
            {
                CreateBuilder().WithPlayer(serverId, out var player1).Connection,
                CreateBuilder().WithPlayer(serverId, out var player2).Connection
            };

            await playerConnections[0].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player1.Id, validVote);
            await playerConnections[1].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Vote, serverId, player2.Id, validVote);

            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);
            playerConnections[1].On<PokerServerViewModel>(Messages.UPDATED, viewModel =>
            {
                actualVotes = viewModel.CurrentSession.Votes.Values.ToList();
                isShown = viewModel.CurrentSession.IsShown;
                awaitResponse.Release();
            });

            // Act
            await playerConnections[1].InvokeAsync(PlanningPokerHubTestBuilder.Endpoints.Show, serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(validVote, actualVotes[0]);
            Assert.Equal(validVote, actualVotes[1]);
            Assert.True(isShown);
        }
    }
}
