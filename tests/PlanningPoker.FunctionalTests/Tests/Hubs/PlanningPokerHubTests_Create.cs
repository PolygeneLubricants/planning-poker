﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PlanningPoker.Shared.ViewModels;
using Xunit;

namespace PlanningPoker.FunctionalTests.Tests.Hubs
{
    public partial class PlanningPokerHubTests
    {
        [Fact]
        public async Task Create_WhenCardSetIsValid_ServerIsCreated()
        {
            // Arrange
            var builder = CreateBuilder();
            var validCardSet = "1,2,3,5,8,13,21,?";

            // Act
            var result = await builder.Connection.InvokeAsync<ServerCreationResult>(PlanningPokerHubTestBuilder.Endpoints.Create, validCardSet);

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
            var result = await builder.Connection.InvokeAsync<ServerCreationResult>(PlanningPokerHubTestBuilder.Endpoints.Create, invalidCardSet);

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
                new object[] { "abc" },
                new object[] { " " },
                new object[] { (string)null },
                new object[] { "1,2,3,abc" },
                new object[] { "a  b  c, 1,2,3" },
            };
    }
}
