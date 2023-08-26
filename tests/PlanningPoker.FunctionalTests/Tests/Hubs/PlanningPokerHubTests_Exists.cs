using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public partial class PlanningPokerHubTests
    {
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
            var wrongServerId = new Guid("859D0069-6F21-4A7D-8D87-987861240142");

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
            var wrongServerId = new Guid("859D0069-6F21-4A7D-8D87-987861240142");

            var builder = CreateBuilder();

            // Act
            var exists = await builder.HubClient.Exists(wrongServerId);

            // Assert
            Assert.False(exists);
        }
    }
}