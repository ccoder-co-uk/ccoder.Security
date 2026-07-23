// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
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

        Token expectedToken = new()
        {
            Id = Guid.NewGuid()
                     .ToString()
                     .Replace(oldValue: "-", newValue: "") + Guid.NewGuid()
                                                                                      .ToString()
                                                                                      .Replace(oldValue: "-", newValue: ""),
            Expires = DateTimeOffset.Now.AddMinutes(minutes: 10),
            Reason = (int)TokenUse.WorkflowExecution,
            UserName = userId
        };

        tokenBrokerMock.Setup(expression: broker => broker.InsertTokenAsync(token:It.IsAny<Token>()))
            .ReturnsAsync(value: expectedToken);

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
    }
}