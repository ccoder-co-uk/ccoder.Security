// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class TokenServiceTests
{
    [Fact]
    public async Task ShouldDeleteExpiredTokensAsync()
    {
        // given
        int expectedDeletedTokenCount = new Random().Next(minValue: 1, maxValue: 100);

        tokenBrokerMock
            .Setup(broker => broker.DeleteExpiredAsync(
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: expectedDeletedTokenCount);

        // when
        int actualDeletedTokenCount = await tokenService.DeleteExpiredAsync();

        // then
        Assert.Equal(expected: expectedDeletedTokenCount, actual: actualDeletedTokenCount);

        tokenBrokerMock.Verify(
expression: broker => broker.DeleteExpiredAsync(
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()),
times: Times.Once);

        tokenBrokerMock.VerifyNoOtherCalls();
    }
}