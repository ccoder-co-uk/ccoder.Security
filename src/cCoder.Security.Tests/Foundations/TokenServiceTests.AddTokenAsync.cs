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
        // given
        string userId = RandomString();

        Token expectedToken = new()
        {
            Id = Guid.NewGuid().ToString().Replace(oldValue: "-", newValue: "") + Guid.NewGuid().ToString().Replace(oldValue: "-", newValue: ""),
            Expires = DateTimeOffset.Now.AddMinutes(minutes: 10),
            Reason = (int)TokenUse.WorkflowExecution,
            UserName = userId
        };

        tokenBrokerMock.Setup(expression: broker => broker.InsertTokenAsync(It.IsAny<Token>())).ReturnsAsync(value: expectedToken);

        // when
        Token actualToken = await tokenService.AddTokenAsync(userId: userId, tokenUse: TokenUse.WorkflowExecution);
        expectedToken.Expires = actualToken.Expires;

        // then
        actualToken.Should().BeEquivalentTo(expectation: expectedToken);
        tokenBrokerMock.Verify(expression: broker => broker.InsertTokenAsync(token: It.IsAny<Token>()), times: Times.Once);
        tokenBrokerMock.VerifyNoOtherCalls();
    }
}