// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class TokenServiceTests
{
    [Fact]
    public async Task ShouldDeleteTokenAsync()
    {
        // given
        Token inputToken = new()
        {
            Id = Guid.NewGuid().ToString().Replace(oldValue: "-", newValue: "") + Guid.NewGuid().ToString().Replace(oldValue: "-", newValue: ""),
            Expires = DateTimeOffset.Now.AddMinutes(minutes: 10),
            Reason = 0,
            UserName = RandomString()
        };

        // when
        await tokenService.DeleteTokenAsync(deletedToken: inputToken);

        // then
        tokenBrokerMock.Verify(expression: broker =>
            broker.DeleteTokenAsync(deletedToken: inputToken),
times: Times.Once);

        tokenBrokerMock.VerifyNoOtherCalls();
    }
}