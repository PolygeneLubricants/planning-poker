using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public partial class PlanningPokerHubTests : IClassFixture<PlanningPokerWebApplicationFactory>
    {
        private readonly PlanningPokerWebApplicationFactory _factory;
        
        public PlanningPokerHubTests(PlanningPokerWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void Ctor_InitializationOk_ConnectionOpenedOk()
        {
            // Arrange
            var connection = CreateBuilder().HubClient;

            // Act

            // Assert
            Assert.NotNull(connection.ConnectionId);
        }

        private PlanningPokerHubTestBuilder CreateBuilder()
        {
            return new PlanningPokerHubTestBuilder(_factory);
        }
    }
}
