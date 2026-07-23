// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            .Returns(value: Array.Empty<SSOUserRole>().AsQueryable());

        userRoleProcessingServiceMock
            .Setup(x => x.AddSSOUserRoleAsync(inputUserRole))
            .ReturnsAsync(value: inputUserRole);

        SSOUserRole actualUserRole = await userRoleOrchestrationService.AddSSOUserRoleAsync(userRole: inputUserRole);

        actualUserRole.Should().BeSameAs(expected: inputUserRole);

        authorizationBrokerMock.Verify(
expression: x => x.UserIsPortalAdminWithPrivilege(It.IsAny<string>()),
times: Times.Never);
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
            .Returns(value: new[] { new SSOUserRole { UserId = "setup-admin" } }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.UserIsPortalAdminWithPrivilege("userrole_create"));

        userRoleProcessingServiceMock
            .Setup(x => x.AddSSOUserRoleAsync(inputUserRole))
            .ReturnsAsync(value: inputUserRole);

        await userRoleOrchestrationService.AddSSOUserRoleAsync(userRole: inputUserRole);

        authorizationBrokerMock.Verify(
expression: x => x.UserIsPortalAdminWithPrivilege("userrole_create"),
times: Times.Once);
    }
}