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
        // Given
        SSORole inputRole = new()
        {
            Name = "Administrators",
            TenantId = "tenant-1"
        };

        roleProcessingServiceMock
            .Setup(expression: x => x.GetAllSSORoles())
            .Returns(value: Array.Empty<SSORole>()
                                .AsQueryable());

        roleProcessingServiceMock
            .Setup(expression: x => x.AddSSORoleAsync(item:inputRole))
            .ReturnsAsync(value: inputRole);

        // When
        SSORole actualRole = await roleOrchestrationService.AddSSORoleAsync(item: inputRole);

        // Then
        actualRole.Should()
            .BeSameAs(expected: inputRole);

        authorizationProcessingServiceMock.Verify(
expression: service => service.EnsureUserIsPortalAdminWithPrivilege(privilege: It.IsAny<string>()),
times: Times.Never);
    }

    [Fact]
    public async Task ShouldRequirePortalAdminCheckAfterBootstrapRoleExists()
    {
        // Given
        SSORole inputRole = new()
        {
            Name = "Editors",
            TenantId = "tenant-1"
        };

        roleProcessingServiceMock
            .Setup(expression: x => x.GetAllSSORoles())
            .Returns(value: new[] { new SSORole { Name = "Administrators", TenantId = "tenant-1" } }.AsQueryable());

        authorizationProcessingServiceMock
            .Setup(expression: service =>
                service.EnsureUserIsPortalAdminWithPrivilege(
                    privilege: "tenant_admin"));

        roleProcessingServiceMock
            .Setup(expression: x => x.AddSSORoleAsync(item:inputRole))
            .ReturnsAsync(value: inputRole);

        // When
        await roleOrchestrationService.AddSSORoleAsync(item: inputRole);

        // Then
        authorizationProcessingServiceMock.Verify(
expression: service => service.EnsureUserIsPortalAdminWithPrivilege(privilege: "tenant_admin"),
times: Times.Once);
    }
}