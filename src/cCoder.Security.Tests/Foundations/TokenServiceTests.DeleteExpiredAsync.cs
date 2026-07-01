using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class TokenServiceTests
{
    [Fact]
    public async Task ShouldDeleteExpiredTokensAsync()
    {
        // given
        int expectedDeletedTokenCount = new Random().Next(1, 100);

        tokenBrokerMock
            .Setup(broker => broker.DeleteExpiredAsync(
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDeletedTokenCount);

        // when
        int actualDeletedTokenCount = await tokenService.DeleteExpiredAsync();

        // then
        Assert.Equal(expectedDeletedTokenCount, actualDeletedTokenCount);

        tokenBrokerMock.Verify(
            broker => broker.DeleteExpiredAsync(
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        tokenBrokerMock.VerifyNoOtherCalls();
    }
}
