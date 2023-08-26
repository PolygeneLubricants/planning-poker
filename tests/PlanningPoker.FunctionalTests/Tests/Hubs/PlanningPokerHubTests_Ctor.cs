using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_Ctor : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_Ctor(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
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
    }
}
