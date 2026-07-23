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
            .Setup(x => x.GetAllSSORoles())
            .Returns(value: Array.Empty<SSORole>().AsQueryable());
        roleProcessingServiceMock
            .Setup(x => x.AddSSORoleAsync(inputRole))
            .ReturnsAsync(value: inputRole);

        SSORole actualRole = await roleOrchestrationService.AddSSORoleAsync(item: inputRole);

        actualRole.Should().BeSameAs(expected: inputRole);
        authorizationBrokerMock.Verify(
expression: x => x.UserIsPortalAdminWithPrivilege(It.IsAny<string>()),
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
            .Setup(x => x.GetAllSSORoles())
            .Returns(value: new[] { new SSORole { Name = "Administrators", TenantId = "tenant-1" } }.AsQueryable());
        authorizationBrokerMock
            .Setup(expression: x => x.UserIsPortalAdminWithPrivilege("tenant_admin"));
        roleProcessingServiceMock
            .Setup(x => x.AddSSORoleAsync(inputRole))
            .ReturnsAsync(value: inputRole);

        await roleOrchestrationService.AddSSORoleAsync(item: inputRole);

        authorizationBrokerMock.Verify(
expression: x => x.UserIsPortalAdminWithPrivilege("tenant_admin"),
times: Times.Once);
    }
}