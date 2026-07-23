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
        // Given
        Token inputToken = new()
        {
            Id = Guid.NewGuid()
                     .ToString()
                     .Replace(oldValue: "-", newValue: "") + Guid.NewGuid()
                                                                                      .ToString()
                                                                                      .Replace(oldValue: "-", newValue: ""),
            Expires = DateTimeOffset.Now.AddMinutes(minutes: 10),
            Reason = 0,
            UserName = RandomString()
        };

        // When
        await tokenService.DeleteTokenAsync(item: inputToken);

        // Then
        tokenBrokerMock.Verify(expression: broker =>
            broker.DeleteTokenAsync(token: inputToken),
times: Times.Once);

        tokenBrokerMock.VerifyNoOtherCalls();
    }
}