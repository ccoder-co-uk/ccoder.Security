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
    public void ShouldGetTokens()
    {
        Token expectedToken = new()
        {
            Id = Guid.NewGuid().ToString().Replace(oldValue: "-", newValue: "") + Guid.NewGuid().ToString().Replace(oldValue: "-", newValue: ""),
            Expires = DateTimeOffset.Now.AddMinutes(minutes: 10),
            Reason = 0,
            UserName = RandomString()
        };

        // given
        tokenBrokerMock.Setup(broker => broker.GetAllTokens(false)).Returns(value: new[] { expectedToken }.AsQueryable());

        // when
        Token actualToken = tokenService.GetAllTokens().First();

        // then
        actualToken.Should().BeEquivalentTo(expectation: expectedToken);
        tokenBrokerMock.Verify(expression: broker => broker.GetAllTokens(false), times: Times.Once);
        tokenBrokerMock.VerifyNoOtherCalls();
    }
}