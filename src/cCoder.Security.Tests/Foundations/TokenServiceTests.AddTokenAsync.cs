// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class TokenServiceTests
{
    [Fact]
    public async Task ShouldAddTokenAsync()
    {
        // Given
        string userId = RandomString();
        string selector = RandomString();
        string secret = RandomString();
        string rawToken = RandomString();
        string secretHash = RandomString();

        Token expectedToken = new()
        {
            Id = rawToken,
            Expires = DateTimeOffset.Now.AddMinutes(minutes: 10),
            Reason = (int)TokenUse.WorkflowExecution,
            UserName = userId
        };

        Token storedToken = new()
        {
            Id = selector,
            Expires = expectedToken.Expires,
            Reason = expectedToken.Reason,
            UserName = expectedToken.UserName,
            SecretHash = secretHash
        };

        tokenGenerationBrokerMock
            .Setup(expression: broker => broker.GenerateSelector())
            .Returns(value: selector);

        tokenGenerationBrokerMock
            .Setup(expression: broker => broker.GenerateSecret())
            .Returns(value: secret);

        tokenGenerationBrokerMock
            .Setup(expression: broker => broker.Combine(
                selector: selector,
                secret: secret))
            .Returns(value: rawToken);

        passwordHashingBrokerMock
            .Setup(expression: broker => broker.HashTokenSecret(secret: secret))
            .Returns(value: secretHash);

        tokenBrokerMock
            .Setup(expression: broker => broker.InsertTokenAsync(
                token: It.Is<Token>(match: token =>
                    token.Id == selector
                    && token.SecretHash == secretHash)))
            .ReturnsAsync(value: storedToken);

        configurationBrokerMock
            .Setup(expression: broker => broker.GetValue(
                section: "Settings",
                key: "TokenTimeout"))
            .Returns(value: null);

        Token actualToken = await tokenService.AddTokenAsync(userId: userId, tokenUse: TokenUse.WorkflowExecution);
        // When
        expectedToken.Expires = actualToken.Expires;

        // Then
        actualToken.Should()
            .BeEquivalentTo(expectation: expectedToken);

        tokenBrokerMock.Verify(expression: broker => broker.InsertTokenAsync(token: It.IsAny<Token>()), times: Times.Once);

        configurationBrokerMock.Verify(
            expression: broker => broker.GetValue(
                section: "Settings",
                key: "TokenTimeout"),
            times: Times.Once);

        tokenBrokerMock.VerifyNoOtherCalls();
        configurationBrokerMock.VerifyNoOtherCalls();
        tokenGenerationBrokerMock.VerifyAll();
        passwordHashingBrokerMock.VerifyAll();
    }
}