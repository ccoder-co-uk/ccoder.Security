using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class SSOUserServiceTests
{
    [Fact]
    public async Task ShouldUpdateSSOUserAsync()
    {
        // given
        SSOUser inputSSOUser = RandomUser(RandomString());
        SSOUser expectedSSOUser = inputSSOUser.DeepClone();

        userBrokerMock.Setup(broker => broker.UpdateSSOUserAsync(inputSSOUser)).ReturnsAsync(expectedSSOUser);

        // when
        SSOUser actualSSOUser = await userService.UpdateSSOUserAsync(inputSSOUser);

        // then
        actualSSOUser.Should().BeEquivalentTo(expectedSSOUser);

        userBrokerMock.Verify(broker => 
            broker.UpdateSSOUserAsync(inputSSOUser), 
            Times.Once);

        userBrokerMock.VerifyNoOtherCalls();
    }
}