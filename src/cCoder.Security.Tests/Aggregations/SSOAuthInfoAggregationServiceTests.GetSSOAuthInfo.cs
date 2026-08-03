// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Configurations;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations;
using cCoder.Security.Services.Processings.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Aggregations;

public sealed partial class SSOAuthInfoAggregationServiceTests
{
    [Theory]
    [InlineData(TokenUse.Auth)]
    [InlineData(TokenUse.WorkflowExecution)]
    public async Task SupportedBearerAuthenticationReturnsTokenUser(
        TokenUse tokenUse)
    {
        // Given
        const string TokenId = "supported-token";
        const string UserId = "authenticated-user";

        Mock<ISessionProcessingService> sessionProcessingServiceMock =
            new(MockBehavior.Strict);

        Mock<ISSOUserProcessingService> ssoUserProcessingServiceMock =
            new(MockBehavior.Strict);

        Mock<ITokenProcessingService> tokenProcessingServiceMock =
            new(MockBehavior.Strict);

        Mock<IRequestProcessingService> requestProcessingServiceMock =
            new(MockBehavior.Strict);

        requestProcessingServiceMock
            .Setup(expression: service =>
                service.GetHeader(key: "Authorization"))
            .Returns(value: $"Bearer {TokenId}");

        tokenProcessingServiceMock
            .Setup(expression: service =>
                service.GetTokenById(tokenId: TokenId))
            .Returns(value: new Token
            {
                Id = TokenId,
                Reason = (int)tokenUse,
                UserName = UserId
            });

        SSOAuthInfoAggregationService service = new(
            sessionService: sessionProcessingServiceMock.Object,
            userService: ssoUserProcessingServiceMock.Object,
            tokenService: tokenProcessingServiceMock.Object,
            requestProcessingService:
                requestProcessingServiceMock.Object);

        // When
        ISSOAuthInfo authInfo = await service.GetSSOAuthInfoAsync();

        // Then
        authInfo.AuthenticationFailed.Should()
            .BeFalse();

        authInfo.SSOUserId.Should()
            .Be(expected: UserId);

        sessionProcessingServiceMock.VerifyNoOtherCalls();
        ssoUserProcessingServiceMock.VerifyNoOtherCalls();
        tokenProcessingServiceMock.VerifyAll();
        requestProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task InvalidBearerAuthenticationDoesNotFallBackToSession()
    {
        // Given
        Mock<ISessionProcessingService> sessionProcessingServiceMock =
            new(MockBehavior.Strict);

        Mock<ISSOUserProcessingService> ssoUserProcessingServiceMock =
            new(MockBehavior.Strict);

        Mock<ITokenProcessingService> tokenProcessingServiceMock =
            new(MockBehavior.Strict);

        Mock<IRequestProcessingService> requestProcessingServiceMock =
            new(MockBehavior.Strict);

        requestProcessingServiceMock
            .Setup(expression: service =>
                service.GetHeader(key: "Authorization"))
            .Returns(value: "Bearer invalid-token");

        tokenProcessingServiceMock
            .Setup(expression: service =>
                service.GetTokenById(tokenId: "invalid-token"))
            .Returns(value: null);

        SSOAuthInfoAggregationService service = new(
            sessionService: sessionProcessingServiceMock.Object,
            userService: ssoUserProcessingServiceMock.Object,
            tokenService: tokenProcessingServiceMock.Object,
            requestProcessingService:
                requestProcessingServiceMock.Object);

        // When
        ISSOAuthInfo authInfo = await service.GetSSOAuthInfoAsync();

        // Then
        authInfo.AuthenticationFailed
            .Should()
            .BeTrue();

        authInfo.SSOUserId
            .Should()
            .BeNull();

        sessionProcessingServiceMock.VerifyNoOtherCalls();
        ssoUserProcessingServiceMock.VerifyNoOtherCalls();
        tokenProcessingServiceMock.VerifyAll();
        requestProcessingServiceMock.VerifyAll();
    }
}