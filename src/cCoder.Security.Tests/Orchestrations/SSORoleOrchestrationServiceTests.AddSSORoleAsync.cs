// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Orchestrations;

public partial class SSORoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldAllowBootstrappingFirstRoleWithoutPortalAdminCheck()
    {
        SSORole inputRole = new()
        {
            Name = "Administrators",
            TenantId = "tenant-1"
        };

        roleProcessingServiceMock
            .Setup(expression: x => x.GetAllSSORoles())
            .Returns(value: Array.Empty<SSORole>().AsQueryable());

        roleProcessingServiceMock
            .Setup(expression: x => x.AddSSORoleAsync(inputRole))
            .ReturnsAsync(value: inputRole);

        SSORole actualRole = await roleOrchestrationService.AddSSORoleAsync(newSSORole: inputRole);

        actualRole.Should().BeSameAs(expected: inputRole);

        authorizationBrokerMock.Verify(
expression: x => x.UserIsPortalAdminWithPrivilege(privilege: It.IsAny<string>()),
times: Times.Never);
    }

    [Fact]
    public async Task ShouldRequirePortalAdminCheckAfterBootstrapRoleExists()
    {
        SSORole inputRole = new()
        {
            Name = "Editors",
            TenantId = "tenant-1"
        };

        roleProcessingServiceMock
            .Setup(expression: x => x.GetAllSSORoles())
            .Returns(value: new[] { new SSORole { Name = "Administrators", TenantId = "tenant-1" } }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.UserIsPortalAdminWithPrivilege(privilege: "tenant_admin"));

        roleProcessingServiceMock
            .Setup(expression: x => x.AddSSORoleAsync(inputRole))
            .ReturnsAsync(value: inputRole);

        await roleOrchestrationService.AddSSORoleAsync(newSSORole: inputRole);

        authorizationBrokerMock.Verify(
expression: x => x.UserIsPortalAdminWithPrivilege(privilege: "tenant_admin"),
times: Times.Once);
    }
}