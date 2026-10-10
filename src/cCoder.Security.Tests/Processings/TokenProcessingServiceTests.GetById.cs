// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings;

public sealed partial class TokenProcessingServiceTests
{
    [Fact]
    public void ShouldReturnCachedTokenWithoutReadingStorage()
    {
        // Given
        const string TokenId = "selector.secret";

        Token cachedToken = new()
        {
            Id = "selector",
            UserName = "user",
            Reason = (int)TokenUse.Auth,
            Expires = DateTimeOffset.Now.AddMinutes(minutes: 1)
        };

        tokenValidationCacheBrokerMock
            .Setup(expression: broker => broker.Get(tokenId: TokenId))
            .Returns(value: cachedToken);

        TokenProcessingService service = CreateService();

        // When
        Token actualToken = service.GetTokenById(tokenId: TokenId);

        // Then
        actualToken.Should()
            .BeSameAs(expected: cachedToken);

        tokenValidationCacheBrokerMock.VerifyAll();
        tokenServiceMock.VerifyNoOtherCalls();
        tokenGenerationBrokerMock.VerifyNoOtherCalls();
        passwordHashingBrokerMock.VerifyNoOtherCalls();
    }

    private TokenProcessingService CreateService() =>
        new(
            tokenService: tokenServiceMock.Object,
            tokenGenerationBroker: tokenGenerationBrokerMock.Object,
            passwordHashingBroker: passwordHashingBrokerMock.Object,
            tokenValidationCacheBroker:
                tokenValidationCacheBrokerMock.Object);
}