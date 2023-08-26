using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_Create : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_Create(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Create_WhenCardSetIsValid_ServerIsCreated()
        {
            // Arrange
            var builder = CreateBuilder();
            var validCardSet = "1,2,3,5,8,13,21,?";

            // Act
            var result = await builder.HubClient.CreateServer(validCardSet);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Created);
            Assert.NotNull(result.ServerId);
            Assert.NotEqual(Guid.Empty, result.ServerId);
            Assert.Null(result.ValidationMessage);
        }

        [Theory]
        [MemberData(nameof(InvalidCardSets))]
        public async Task Create_WhenCardSetIsInvalid_ValidationError(string invalidCardSet)
        {
            // Arrange
            var builder = CreateBuilder();

            // Act
            var result = await builder.HubClient.CreateServer(invalidCardSet);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Created);
            Assert.Null(result.ServerId);
            Assert.NotNull(result.ValidationMessage);
        }

        public static IEnumerable<object[]> InvalidCardSets =>
            new List<object[]>
            {
                new object[] { "" },
                new object[] { "abcde" },
                new object[] { " " },
                new object[] { (string)null },
                new object[] { "1,2,3,abcde" },
                new object[] { "a  b  c, 1,2,3" },
            };
    }
}
