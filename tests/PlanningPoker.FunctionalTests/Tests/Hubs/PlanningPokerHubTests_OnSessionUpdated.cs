using System;
using System.Threading;
using System.Threading.Tasks;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public partial class PlanningPokerHubTests
    {
        [Fact]
        public async Task OnSessionUpdated_StateOncePlayerJoins_StateUpdatedCorrectly()
        {
            // Arrange
            var cardSet = "1,2,3,5,8,13,21,?";
            var builder = CreateBuilder();
            builder.WithServer(cardSet, out var serverId);
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);

            PokerServerViewModel? updatedSession = null;
            builder.HubClient.OnSessionUpdated(viewModel =>
            {
                updatedSession = viewModel;
                awaitResponse.Release();
            });

            // Act
            builder.WithPlayer(serverId, out var player);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));
            
            Assert.NotNull(updatedSession);
            Assert.Equal(serverId, updatedSession.Id);

            // Assert players state
            Assert.Equal(1, updatedSession.Players.Count);
            Assert.Null(updatedSession.Players[0].Id);
            Assert.Equal(player.PublicId, updatedSession.Players[0].PublicId);
            Assert.Equal(player.Name, updatedSession.Players[0].Name);
            Assert.Equal(player.Type, updatedSession.Players[0].Type);

            // Assert session state
            Assert.NotNull(updatedSession.CurrentSession);
            Assert.False(updatedSession.CurrentSession.CanClear);
            Assert.False(updatedSession.CurrentSession.CanShow);
            Assert.True(updatedSession.CurrentSession.CanVote);
            Assert.False(updatedSession.CurrentSession.IsShown);
            Assert.NotNull(updatedSession.CurrentSession.CardSet);
            Assert.NotEmpty(updatedSession.CurrentSession.CardSet);
            Assert.Equal(8, updatedSession.CurrentSession.CardSet.Count);
            Assert.Equal("1",  updatedSession.CurrentSession.CardSet[0]);
            Assert.Equal("2",  updatedSession.CurrentSession.CardSet[1]);
            Assert.Equal("3",  updatedSession.CurrentSession.CardSet[2]);
            Assert.Equal("5",  updatedSession.CurrentSession.CardSet[3]);
            Assert.Equal("8",  updatedSession.CurrentSession.CardSet[4]);
            Assert.Equal("13", updatedSession.CurrentSession.CardSet[5]);
            Assert.Equal("21", updatedSession.CurrentSession.CardSet[6]);
            Assert.Equal("?",  updatedSession.CurrentSession.CardSet[7]);
            Assert.Empty(updatedSession.CurrentSession.Votes);
        }

        [Fact]
        public async Task OnSessionUpdated_StateOncePlayerVotesAndIsShown_StateUpdatedCorrectly()
        {
            // Arrange
            var cardSet = "1,2,3,5,8,13,21,?";
            var validVote = "8";
            var builder = CreateBuilder();
            builder.WithServer(cardSet, out var serverId);
            builder.WithPlayer(serverId, out var player);
            builder.WithPlayerVoted(serverId, player.Id, validVote);
            SemaphoreSlim awaitResponse = new SemaphoreSlim(0);

            PokerServerViewModel? updatedSession = null;
            builder.HubClient.OnSessionUpdated(viewModel =>
            {
                updatedSession = viewModel;
                awaitResponse.Release();
            });

            // Act
            builder.WithVotesShown(serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.NotNull(updatedSession);
            Assert.Equal(serverId, updatedSession.Id);

            // Assert players state
            Assert.Equal(1, updatedSession.Players.Count);
            Assert.Null(updatedSession.Players[0].Id);
            Assert.Equal(player.PublicId, updatedSession.Players[0].PublicId);
            Assert.Equal(player.Name, updatedSession.Players[0].Name);
            Assert.Equal(player.Type, updatedSession.Players[0].Type);

            // Assert session state
            Assert.NotNull(updatedSession.CurrentSession);
            Assert.True(updatedSession.CurrentSession.CanClear);
            Assert.False(updatedSession.CurrentSession.CanShow);
            Assert.False(updatedSession.CurrentSession.CanVote);
            Assert.True(updatedSession.CurrentSession.IsShown);
            Assert.NotNull(updatedSession.CurrentSession.CardSet);
            Assert.NotEmpty(updatedSession.CurrentSession.CardSet);
            Assert.Equal(8, updatedSession.CurrentSession.CardSet.Count);
            Assert.Equal("1", updatedSession.CurrentSession.CardSet[0]);
            Assert.Equal("2", updatedSession.CurrentSession.CardSet[1]);
            Assert.Equal("3", updatedSession.CurrentSession.CardSet[2]);
            Assert.Equal("5", updatedSession.CurrentSession.CardSet[3]);
            Assert.Equal("8", updatedSession.CurrentSession.CardSet[4]);
            Assert.Equal("13", updatedSession.CurrentSession.CardSet[5]);
            Assert.Equal("21", updatedSession.CurrentSession.CardSet[6]);
            Assert.Equal("?", updatedSession.CurrentSession.CardSet[7]);
            Assert.Equal(1, updatedSession.CurrentSession.Votes.Count);
            Assert.NotNull(updatedSession.CurrentSession.Votes[player.PublicId.ToString()]);
            Assert.Equal(validVote, updatedSession.CurrentSession.Votes[player.PublicId.ToString()]);
        }

        [Fact]
        public async Task OnSessionUpdated_WhenPlayerClearVotes_StateUpdatedCorrectly()
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

            PokerServerViewModel? updatedSession = null;
            builder.HubClient.OnSessionUpdated(viewModel =>
            {
                updatedSession = viewModel;
                awaitResponse.Release();
            });

            // Act
            builder.WithVotesCleared(serverId);

            // Assert
            await awaitResponse.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.NotNull(updatedSession);
            Assert.Equal(serverId, updatedSession.Id);

            // Assert players state
            Assert.Equal(1, updatedSession.Players.Count);
            Assert.Null(updatedSession.Players[0].Id);
            Assert.Equal(player.PublicId, updatedSession.Players[0].PublicId);
            Assert.Equal(player.Name, updatedSession.Players[0].Name);
            Assert.Equal(player.Type, updatedSession.Players[0].Type);

            // Assert session state
            Assert.NotNull(updatedSession.CurrentSession);
            Assert.False(updatedSession.CurrentSession.CanClear);
            Assert.False(updatedSession.CurrentSession.CanShow);
            Assert.True(updatedSession.CurrentSession.CanVote);
            Assert.False(updatedSession.CurrentSession.IsShown);
            Assert.NotNull(updatedSession.CurrentSession.CardSet);
            Assert.NotEmpty(updatedSession.CurrentSession.CardSet);
            Assert.Equal(8, updatedSession.CurrentSession.CardSet.Count);
            Assert.Empty(updatedSession.CurrentSession.Votes);
        }
    }
}