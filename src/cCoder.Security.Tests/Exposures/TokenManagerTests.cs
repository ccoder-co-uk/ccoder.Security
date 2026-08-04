// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Exposures;

public sealed partial class TokenManagerTests
{
    [Fact]
    public async Task ShouldDelegateTokenIssuingToAuthenticationServiceAsync()
    {
        // Given
        string userId = Guid.NewGuid()
            .ToString();

        TokenUse tokenUse = TokenUse.Auth;
        var expectedToken = new Token();

        var authenticationAggregationServiceMock =
            new Mock<IAuthenticationAggregationService>(behavior: MockBehavior.Strict);

        authenticationAggregationServiceMock
            .Setup(expression: service => service.IssueTokenAsync(
                userId: userId,
                tokenUse: tokenUse))
            .Returns(value: new ValueTask<Token>(result: expectedToken));

        var tokenManager = new TokenManager(
            authenticationAggregationService: authenticationAggregationServiceMock.Object);

        // When
        Token actualToken = await tokenManager.IssueTokenAsync(
            userId: userId,
            tokenUse: tokenUse);

        // Then
        actualToken.Should()
            .BeSameAs(expected: expectedToken);

        authenticationAggregationServiceMock.Verify(
            expression: service => service.IssueTokenAsync(
                userId: userId,
                tokenUse: tokenUse),
            times: Times.Once);

        authenticationAggregationServiceMock.VerifyNoOtherCalls();
    }
}