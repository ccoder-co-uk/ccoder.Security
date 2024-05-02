using cCoder.Security.Objects.Entities;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class SSOUserRoleServiceTests
{
    [Fact]
    public async void ShouldDeleteSSOUserRoleAsync()
    {
        // given
        SSOUserRole inputSSOUserRole = RandomUserRole();
        SSOUserRole expectedSSOUserRole = inputSSOUserRole.DeepClone();

        // when
        await userRoleService.DeleteSSOUserRoleAsync(inputSSOUserRole);

        // then
        userRoleBrokerMock.Verify(broker => 
            broker.DeleteSSOUserRoleAsync(inputSSOUserRole), 
            Times.Once);

        userRoleBrokerMock.VerifyNoOtherCalls();
    }
}