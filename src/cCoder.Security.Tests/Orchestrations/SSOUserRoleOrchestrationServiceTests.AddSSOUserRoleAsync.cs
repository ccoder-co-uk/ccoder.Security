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
            .Setup(expression: x => x.GetAllSSOUserRoles())
            .Returns(value: Array.Empty<SSOUserRole>().AsQueryable());

        userRoleProcessingServiceMock
            .Setup(expression: x => x.AddSSOUserRoleAsync(inputUserRole))
            .ReturnsAsync(value: inputUserRole);

        SSOUserRole actualUserRole = await userRoleOrchestrationService.AddSSOUserRoleAsync(userRole: inputUserRole);

        actualUserRole.Should().BeSameAs(expected: inputUserRole);

        authorizationProcessingServiceMock.Verify(
expression: service => service.EnsureUserIsPortalAdminWithPrivilege(privilege: It.IsAny<string>()),
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
            .Setup(expression: x => x.GetAllSSOUserRoles())
            .Returns(value: new[] { new SSOUserRole { UserId = "setup-admin" } }.AsQueryable());

        authorizationProcessingServiceMock
            .Setup(expression: service =>
                service.EnsureUserIsPortalAdminWithPrivilege(
                    privilege: "userrole_create"));

        userRoleProcessingServiceMock
            .Setup(expression: x => x.AddSSOUserRoleAsync(inputUserRole))
            .ReturnsAsync(value: inputUserRole);

        await userRoleOrchestrationService.AddSSOUserRoleAsync(userRole: inputUserRole);

        authorizationProcessingServiceMock.Verify(
expression: service => service.EnsureUserIsPortalAdminWithPrivilege(privilege: "userrole_create"),
times: Times.Once);
    }
}