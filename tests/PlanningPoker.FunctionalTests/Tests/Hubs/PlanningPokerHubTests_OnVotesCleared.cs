using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    [Collection("PlanningPokerHubTests")]
    public class PlanningPokerHubTests_OnVotesCleared : PlanningPokerHubTestFixture
    {
        public PlanningPokerHubTests_OnVotesCleared(PlanningPokerWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task OnVotesCleared_WhenPlayerClearVotes_VotesClearedSent()
        {
            // Arrange
            var cardSet = "1,2,3,5,8,13,21,?";
            var validVote = "8";
            var builder = CreateBuilder();
            builder.WithServer(cardSet, out var serverId);
            builder.WithPlayer(serverId, out var player);
            builder.WithPlayerVoted(serverId, player.Id, validVote);
            builder.WithVotesShown(serverId);
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);

            var votesClearedSent = false;
            builder.HubClient.OnVotesCleared(() =>
            {
                votesClearedSent = true;
                awaitResponse.Release();
            });

            // Act
            builder.WithVotesCleared(serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.True(votesClearedSent);
        }
    }
}