using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_Exists : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_Exists(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Exists_WhenServerExists_ReturnsTrue()
        {
            // Arrange
            var builder = CreateBuilder();
            builder.WithServer(out var createdServerId);

            // Act
            var exists = await builder.HubClient.Exists(createdServerId);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task Exists_WhenServerDoesNotExists_ReturnsFalse()
        {
            // Arrange
            var wrongServerId = Guid.NewGuid();

            var builder = CreateBuilder();
            builder.WithServer(out _);

            // Act
            var exists = await builder.HubClient.Exists(wrongServerId);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task Exists_WhenServerNoServersExist_ReturnsFalse()
        {
            // Arrange
            var wrongServerId = Guid.NewGuid();

            var builder = CreateBuilder();

            // Act
            var exists = await builder.HubClient.Exists(wrongServerId);

            // Assert
            Assert.False(exists);
        }
    }
}