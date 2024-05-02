using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class TokenServiceTests
{
    [Fact]
    public async void ShouldDeleteTokenAsync()
    {
        // given
        Token inputToken = new()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", ""),
            Expires = DateTimeOffset.Now.AddMinutes(10),
            Reason = 0,
            UserName = RandomString()
        };

        // when
        await tokenService.DeleteTokenAsync(inputToken);

        // then
        tokenBrokerMock.Verify(broker => 
            broker.DeleteTokenAsync(inputToken), 
            Times.Once);

        tokenBrokerMock.VerifyNoOtherCalls();
    }
}