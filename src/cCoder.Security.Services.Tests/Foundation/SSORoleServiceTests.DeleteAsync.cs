using cCoder.Security.Objects.Entities;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class SSORoleServiceTests
{
    [Fact]
    public async void ShouldDeleteSSORoleAsync()
    {
        // given
        SSORole inputSSORole = RandomRole(Guid.NewGuid());
        SSORole expectedSSORole = inputSSORole.DeepClone();

        // when
        await roleService.DeleteSSORoleAsync(inputSSORole);

        // then
        roleBrokerMock.Verify(broker => 
            broker.DeleteSSORoleAsync(inputSSORole), 
            Times.Once);

        roleBrokerMock.VerifyNoOtherCalls();
    }
}