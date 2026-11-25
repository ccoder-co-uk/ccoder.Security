// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

using FluentAssertions;

using Moq;

using Xunit;

namespace cCoder.Security.Tests.Orchestrations;

public partial class SSORoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldRequireTenantAdminPrivilegeForTenantRoleUpdate()
    {
        // Given
        SSORole inputRole = new()
        {
            Id = Guid.NewGuid(),
            TenantId = "tenant-1"
        };

        authorizationProcessingServiceMock
            .Setup(expression: service => service.EnsureUserHasPrivilege(
                privilege: "tenant_admin",
                tenantId: inputRole.TenantId));

        roleProcessingServiceMock
            .Setup(expression: service => service.UpdateSSORoleAsync(item: inputRole))
            .ReturnsAsync(value: inputRole);

        // When
        SSORole actualRole = await roleOrchestrationService.UpdateSSORoleAsync(
            item: inputRole);

        // Then
        actualRole
            .Should()
            .BeSameAs(expected: inputRole);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.EnsureUserHasPrivilege(
                privilege: "tenant_admin",
                tenantId: inputRole.TenantId),
            times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.EnsureUserIsPortalAdminWithPrivilege(
                privilege: "tenant_admin"),
            times: Times.Never);
    }

    [Fact]
    public async Task ShouldRequireTenantAdminPrivilegeForTenantRoleDelete()
    {
        // Given
        SSORole inputRole = new()
        {
            Id = Guid.NewGuid(),
            TenantId = "tenant-1"
        };

        authorizationProcessingServiceMock
            .Setup(expression: service => service.EnsureUserHasPrivilege(
                privilege: "tenant_admin",
                tenantId: inputRole.TenantId));

        roleProcessingServiceMock
            .Setup(expression: service => service.DeleteSSORoleAsync(item: inputRole))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await roleOrchestrationService.DeleteSSORoleAsync(item: inputRole);

        // Then
        authorizationProcessingServiceMock.Verify(
            expression: service => service.EnsureUserHasPrivilege(
                privilege: "tenant_admin",
                tenantId: inputRole.TenantId),
            times: Times.Once);

        authorizationProcessingServiceMock.Verify(
            expression: service => service.EnsureUserIsPortalAdminWithPrivilege(
                privilege: "tenant_admin"),
            times: Times.Never);
    }
}