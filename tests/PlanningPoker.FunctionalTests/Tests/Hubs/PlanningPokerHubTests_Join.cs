using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanningPoker.Hub.Client.Abstractions;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_Join : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_Join(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Join_WhenServerIsMissing_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();
            var expectedPlayerName = "playerName1";
            var expectedPlayerType = PlayerType.Participant;
            var recoveryId = Guid.NewGuid();

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.JoinServer(Guid.NewGuid(), recoveryId, expectedPlayerName, expectedPlayerType));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Join_WhenServerExists_PlayerHasJoined()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            var expectedPlayerName = "playerName1";
            var expectedPlayerType = PlayerType.Participant;
            var recoveryId = Guid.NewGuid();

            // Act
            var result = await builder.HubClient.JoinServer(serverId, recoveryId, expectedPlayerName, expectedPlayerType);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(builder.HubClient.ConnectionId, result.Id);
            Assert.Equal(expectedPlayerName, result.Name);
            Assert.Equal(expectedPlayerType, result.Type);
            Assert.Equal(0, result.PublicId);
        }

        [Fact]
        public async Task Join_WhenServerExistsAndMultiplePlayersJoin_AllPlayersHasJoined()
        {
            // Arrange
            var builder = CreateBuilder();
            var playerConnections = new List<IPlanningPokerHubClient>
            {
                CreateBuilder().HubClient,
                CreateBuilder().HubClient,
                CreateBuilder().HubClient
            };

            builder.WithServer(out var serverId);
            var expectedPlayerNames = new string[]
            {
                "player1",
                "player2",
                "player3",
            };
            var expectedPlayerType = PlayerType.Participant;

            // Act
            var results = new List<PlayerViewModel?>();
            for (var i = 0; i < playerConnections.Count; i++)
            {
                results.Add(await playerConnections[i].JoinServer(serverId, Guid.NewGuid(), expectedPlayerNames[i], expectedPlayerType));
            }

            // Assert
            Assert.All(results, Assert.NotNull);
            for (var i = 0; i < playerConnections.Count; i++)
            {
                Assert.Equal(playerConnections[i].   ConnectionId, results[i].Id);
                Assert.Equal(expectedPlayerNames[i], results[i].Name);
                Assert.Equal(expectedPlayerType,     results[i].Type);
                Assert.Equal(i,                      results[i].PublicId);
            }
        }

        [Fact]
        public async Task Join_WhenPlayerNameIsMissing_ReturnsError()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            var expectedPlayerName = "";
            var expectedPlayerType = PlayerType.Participant;
            var recoveryId = Guid.NewGuid();

            // Act
            var exceptionRecord = await Record.ExceptionAsync(() => builder.HubClient.JoinServer(serverId, recoveryId, expectedPlayerName, expectedPlayerType));

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public async Task Join_WhenPlayerIsObserver_ObserverHasJoined()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var serverId);
            var expectedPlayerName = "observerName1";
            var expectedPlayerType = PlayerType.Observer;
            var recoveryId = Guid.NewGuid();

            // Act
            var result = await builder.HubClient.JoinServer(serverId, recoveryId, expectedPlayerName, expectedPlayerType);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(builder.HubClient.ConnectionId, result.Id);
            Assert.Equal(expectedPlayerName, result.Name);
            Assert.Equal(expectedPlayerType, result.Type);
            Assert.Equal(0, result.PublicId);
        }
    }
}