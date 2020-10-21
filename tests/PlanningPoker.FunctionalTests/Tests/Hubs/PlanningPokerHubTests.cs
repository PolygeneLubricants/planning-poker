using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
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
            var connection = CreateBuilder().Connection;

            // Act

            // Assert
            Assert.Equal(HubConnectionState.Connected, connection.State);
            Assert.NotNull(connection.ConnectionId);
        }

        private PlanningPokerHubTestBuilder CreateBuilder()
        {
            return new PlanningPokerHubTestBuilder(_factory);
        }
    }
}
