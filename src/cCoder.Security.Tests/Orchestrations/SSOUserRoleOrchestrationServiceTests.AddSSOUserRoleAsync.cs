using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Orchestrations;

public partial class SSOUserRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldAllowBootstrappingFirstUserRoleWithoutPortalAdminCheck()
    {
        SSOUserRole inputUserRole = new()
        {
            UserId = "setup-admin"
        };

        userRoleProcessingServiceMock
            .Setup(x => x.GetAllSSOUserRoles())
            .Returns(Array.Empty<SSOUserRole>().AsQueryable());
        userRoleProcessingServiceMock
            .Setup(x => x.AddSSOUserRoleAsync(inputUserRole))
            .ReturnsAsync(inputUserRole);

        SSOUserRole actualUserRole = await userRoleOrchestrationService.AddSSOUserRoleAsync(inputUserRole);

        actualUserRole.Should().BeSameAs(inputUserRole);
        authorizationBrokerMock.Verify(
            x => x.UserIsPortalAdminWithPrivilege(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ShouldRequirePortalAdminCheckAfterBootstrapUserRoleExists()
    {
        SSOUserRole inputUserRole = new()
        {
            UserId = "other-admin"
        };

        userRoleProcessingServiceMock
            .Setup(x => x.GetAllSSOUserRoles())
            .Returns(new[] { new SSOUserRole { UserId = "setup-admin" } }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.UserIsPortalAdminWithPrivilege("userrole_create"));
        userRoleProcessingServiceMock
            .Setup(x => x.AddSSOUserRoleAsync(inputUserRole))
            .ReturnsAsync(inputUserRole);

        await userRoleOrchestrationService.AddSSOUserRoleAsync(inputUserRole);

        authorizationBrokerMock.Verify(
            x => x.UserIsPortalAdminWithPrivilege("userrole_create"),
            Times.Once);
    }
}

