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
            Id = Guid.NewGuid().ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", ""),
            Expires = DateTimeOffset.Now.AddMinutes(10),
            Reason = 0,
            UserName = RandomString()
        };

        // given
        tokenBrokerMock.Setup(broker => broker.GetAllTokens(false)).Returns(new[] { expectedToken }.AsQueryable());

        // when
        Token actualToken = tokenService.GetAllTokens().First();

        // then
        actualToken.Should().BeEquivalentTo(expectedToken);
        tokenBrokerMock.Verify(broker => broker.GetAllTokens(false), Times.Once);
        tokenBrokerMock.VerifyNoOtherCalls();
    }
}
