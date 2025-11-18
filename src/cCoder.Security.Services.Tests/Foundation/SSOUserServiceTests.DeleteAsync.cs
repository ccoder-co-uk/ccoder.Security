using cCoder.Security.Objects.Entities;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class SSOUserServiceTests
{
    [Fact]
    public async Task ShouldDeleteSSOUserAsync()
    {
        // given
        SSOUser inputSSOUser = RandomUser(RandomString());
        SSOUser expectedSSOUser = inputSSOUser.DeepClone();

        // when
        await userService.DeleteSSOUserAsync(inputSSOUser);

        // then
        userBrokerMock.Verify(broker => 
            broker.DeleteSSOUserAsync(inputSSOUser), 
            Times.Once);

        userBrokerMock.VerifyNoOtherCalls();
    }
}