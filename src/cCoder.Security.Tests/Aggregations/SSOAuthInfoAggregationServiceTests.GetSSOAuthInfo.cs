// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Configurations;
using cCoder.Security.Services.Aggregations;
using cCoder.Security.Services.Processings.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Aggregations;

public sealed partial class SSOAuthInfoAggregationServiceTests
{
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