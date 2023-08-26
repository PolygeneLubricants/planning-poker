using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public abstract class PlanningPokerHubTestFixture : IClassFixture<PlanningPokerWebApplicationFactory>
    {
        protected readonly PlanningPokerWebApplicationFactory _factory;

        protected PlanningPokerHubTestFixture(PlanningPokerWebApplicationFactory factory)
        {
            _factory = factory;
        }

        protected PlanningPokerHubTestBuilder CreateBuilder()
        {
            return new PlanningPokerHubTestBuilder(_factory);
        }
    }
}
