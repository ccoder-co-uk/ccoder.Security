using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class SSOUserRoleServiceTests
{
    [Fact]
    public void ShouldGetSSOUserRolesAsync()
    {
        // given
        IQueryable<SSOUserRole> expectedSSOUserRoles = RandomUserRoles();
        userRoleBrokerMock.Setup(broker => broker.GetAllSSOUserRoles()).Returns(expectedSSOUserRoles);

        // when
        IEnumerable<SSOUserRole> actualSSOUserRoles = userRoleService.GetAllSSOUserRoles();

        // then
        actualSSOUserRoles.Should().BeEquivalentTo(expectedSSOUserRoles);
        userRoleBrokerMock.Verify(broker => broker.GetAllSSOUserRoles(), Times.Once);
        userRoleBrokerMock.VerifyNoOtherCalls();
    }
}