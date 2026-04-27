using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSOUserRoleServiceTests
{
    [Fact]
    public async Task ShouldAddSSOUserRoleAsync()
    {
        // given
        SSOUserRole inputSSOUserRole = RandomUserRole();
        SSOUserRole expectedSSOUserRole = inputSSOUserRole.DeepClone();

        userRoleBrokerMock.Setup(broker => broker.GetAllSSOUserRoles()).Returns(Array.Empty<SSOUserRole>().AsQueryable());
        SSOUserRole submitted = null;
        userRoleBrokerMock
            .Setup(broker => broker.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()))
            .Callback<SSOUserRole>(candidate => submitted = candidate)
            .ReturnsAsync(expectedSSOUserRole);

        // when
        SSOUserRole actualSSOUserRole = await userRoleService.AddSSOUserRoleAsync(inputSSOUserRole);

        // then
        actualSSOUserRole.Should().BeSameAs(inputSSOUserRole);
        submitted.Should().NotBeSameAs(inputSSOUserRole);
        actualSSOUserRole.Should().NotBeSameAs(submitted);
        actualSSOUserRole.Should().BeEquivalentTo(expectedSSOUserRole);

        userRoleBrokerMock.Verify(broker => 
            broker.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()), 
            Times.Once);

        userRoleBrokerMock.VerifyNoOtherCalls();
    }
}


