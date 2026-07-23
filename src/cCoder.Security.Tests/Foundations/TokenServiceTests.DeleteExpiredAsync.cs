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
        // Given
        int expectedDeletedTokenCount = new Random().Next(minValue: 1, maxValue: 100);

        tokenBrokerMock
            .Setup(expression: broker => broker.DeleteExpiredAsync(
expiresBefore:                It.IsAny<DateTimeOffset>(),
cancellationToken:                It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: expectedDeletedTokenCount);

        // When
        int actualDeletedTokenCount = await tokenService.DeleteExpiredAsync();

        // Then
        Assert.Equal(expected: expectedDeletedTokenCount, actual: actualDeletedTokenCount);

        tokenBrokerMock.Verify(
expression: broker => broker.DeleteExpiredAsync(
expiresBefore: It.IsAny<DateTimeOffset>(),
cancellationToken: It.IsAny<CancellationToken>()),
times: Times.Once);

        tokenBrokerMock.VerifyNoOtherCalls();
    }
}