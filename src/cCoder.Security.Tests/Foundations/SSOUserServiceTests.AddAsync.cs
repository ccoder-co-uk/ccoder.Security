using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSOUserServiceTests
{
    [Fact]
    public async Task ShouldAddSSOUserAsync()
    {
        // given
        SSOUser inputSSOUser = RandomUser(RandomString());
        SSOUser expectedSSOUser = inputSSOUser.DeepClone();

        SSOUser submitted = null;
        userBrokerMock
            .Setup(broker => broker.AddSSOUserAsync(It.IsAny<SSOUser>()))
            .Callback<SSOUser>(candidate => submitted = candidate)
            .ReturnsAsync(expectedSSOUser);

        // when
        SSOUser actualSSOUser = await userService.AddSSOUserAsync(inputSSOUser);

        // then
        actualSSOUser.Should().BeSameAs(inputSSOUser);
        submitted.Should().NotBeSameAs(inputSSOUser);
        actualSSOUser.Should().NotBeSameAs(submitted);
        actualSSOUser.Should().BeEquivalentTo(expectedSSOUser);

        userBrokerMock.Verify(broker => broker.AddSSOUserAsync(It.IsAny<SSOUser>()), Times.Once);
        userBrokerMock.VerifyNoOtherCalls();
    }
}


