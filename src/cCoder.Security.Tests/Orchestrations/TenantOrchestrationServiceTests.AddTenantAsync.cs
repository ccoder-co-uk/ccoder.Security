// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.Exceptions;
using cCoder.Security.Objects.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Orchestrations;

public partial class TenantOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldBootstrapFirstTenantWithPortalAdministratorsRoleWithoutMembershipWhenBootstrapUserIsMissing()
    {
        Tenant inputTenant = new()
        {
            Id = "tenant-1",
            Name = "Tenant One",
            CreatedBy = "setup-admin"
        };

        tenantProcessingServiceMock
            .Setup(expression: x => x.GetAllTenants())
            .Returns(value: Array.Empty<Tenant>().AsQueryable());

        tenantProcessingServiceMock
            .Setup(expression: x => x.AddTenantAsync(inputTenant))
            .Returns(value: new ValueTask<Tenant>(inputTenant));

        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetAuthorizationContext())
            .Returns(value: new AuthorizationContext
            {
                CurrentUser = new SSOUser { Id = "Guest" },
                Privileges = new[]
                {
                    new SSOPrivilege { Id = "tenant_read" },
                    new SSOPrivilege { Id = "tenant_admin" },
                    new SSOPrivilege { Id = "tenant_create" }
                }
            });

        roleOrchestrationServiceMock
            .Setup(expression: x => x.AddSSORoleAsync(It.IsAny<SSORole>()))
            .ReturnsAsync(valueFunction: (SSORole role) => role);

        userProcessingServiceMock
            .Setup(expression: x => x.FindById("setup-admin"))
            .Returns(value: (SSOUser)null);

        Tenant result = await tenantOrchestrationService.AddTenantAsync(item: inputTenant);

        result.Should().BeSameAs(expected: inputTenant);

        roleOrchestrationServiceMock.Verify(
expression: x => x.AddSSORoleAsync(item: It.Is<SSORole>(role =>
                role.Name == "Administrators"
                && role.UsersArePortalAdmins
                && role.TenantId == inputTenant.Id
                && role.Privs.Contains("tenant_create"))),
times: Times.Once);

        userRoleOrchestrationServiceMock.Verify(
expression: x => x.AddSSOUserRoleAsync(userRole: It.IsAny<SSOUserRole>()),
times: Times.Never);

        authorizationProcessingServiceMock.Verify(
            expression: service =>
                service.EnsureUserIsPortalAdminWithPrivilege(
                    privilege: It.IsAny<string>()),
            times: Times.Never);
    }

    [Fact]
    public async Task ShouldBootstrapFirstTenantAndAttachMembershipWhenBootstrapUserAlreadyExists()
    {
        Tenant inputTenant = new()
        {
            Id = "tenant-1",
            Name = "Tenant One",
            CreatedBy = "setup-admin"
        };

        tenantProcessingServiceMock
            .Setup(expression: x => x.GetAllTenants())
            .Returns(value: Array.Empty<Tenant>().AsQueryable());

        tenantProcessingServiceMock
            .Setup(expression: x => x.AddTenantAsync(inputTenant))
            .Returns(value: new ValueTask<Tenant>(inputTenant));

        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetAuthorizationContext())
            .Returns(value: new AuthorizationContext
            {
                CurrentUser = new SSOUser { Id = "Guest" },
                Privileges = new[]
                {
                    new SSOPrivilege { Id = "tenant_read" },
                    new SSOPrivilege { Id = "tenant_admin" },
                    new SSOPrivilege { Id = "tenant_create" }
                }
            });

        roleOrchestrationServiceMock
            .Setup(expression: x => x.AddSSORoleAsync(It.IsAny<SSORole>()))
            .ReturnsAsync(valueFunction: (SSORole role) => role);

        userProcessingServiceMock
            .Setup(expression: x => x.FindById("setup-admin"))
            .Returns(value: new SSOUser { Id = "setup-admin" });

        userRoleOrchestrationServiceMock
            .Setup(expression: x => x.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()))
            .ReturnsAsync(valueFunction: (SSOUserRole userRole) => userRole);

        await tenantOrchestrationService.AddTenantAsync(item: inputTenant);

        userRoleOrchestrationServiceMock.Verify(
expression: x => x.AddSSOUserRoleAsync(userRole: It.Is<SSOUserRole>(userRole =>
                userRole.UserId == "setup-admin")),
times: Times.Once);
    }

    [Fact]
    public async Task ShouldRequireCreatedByWhenBootstrappingFirstTenantWithoutAuthenticatedUser()
    {
        Tenant inputTenant = new()
        {
            Id = "tenant-1",
            Name = "Tenant One",
            CreatedBy = string.Empty
        };

        tenantProcessingServiceMock
            .Setup(expression: x => x.GetAllTenants())
            .Returns(value: Array.Empty<Tenant>().AsQueryable());

        tenantProcessingServiceMock
            .Setup(expression: x => x.AddTenantAsync(inputTenant))
            .Returns(value: new ValueTask<Tenant>(inputTenant));

        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetAuthorizationContext())
            .Returns(value: new AuthorizationContext
            {
                CurrentUser = new SSOUser { Id = "Guest" },
                Privileges = Array.Empty<SSOPrivilege>()
            });

        Func<Task> act = async () => await tenantOrchestrationService.AddTenantAsync(item: inputTenant);

        await act.Should()
            .ThrowAsync<SecurityOrchestrationServiceException>();
    }

    [Fact]
    public async Task ShouldRequirePortalAdminForSubsequentTenants()
    {
        Tenant inputTenant = new()
        {
            Id = "tenant-2",
            Name = "Tenant Two",
            CreatedBy = "existing-admin"
        };

        tenantProcessingServiceMock
            .Setup(expression: x => x.GetAllTenants())
            .Returns(value: new[] { new Tenant { Id = "tenant-1", Name = "Existing" } }.AsQueryable());

        authorizationProcessingServiceMock
            .Setup(expression: service =>
                service.EnsureUserIsPortalAdminWithPrivilege(
                    privilege: "tenant_create"));

        tenantProcessingServiceMock
            .Setup(expression: x => x.AddTenantAsync(inputTenant))
            .Returns(value: new ValueTask<Tenant>(inputTenant));

        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetAuthorizationContext())
            .Returns(value: new AuthorizationContext
            {
                CurrentUser = new SSOUser { Id = "existing-admin" },
                Privileges = Array.Empty<SSOPrivilege>()
            });

        roleOrchestrationServiceMock
            .Setup(expression: x => x.AddSSORoleAsync(It.IsAny<SSORole>()))
            .ReturnsAsync(valueFunction: (SSORole role) => role);

        userProcessingServiceMock
            .Setup(expression: x => x.FindById("existing-admin"))
            .Returns(value: new SSOUser { Id = "existing-admin" });

        userRoleOrchestrationServiceMock
            .Setup(expression: x => x.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()))
            .ReturnsAsync(valueFunction: (SSOUserRole userRole) => userRole);

        await tenantOrchestrationService.AddTenantAsync(item: inputTenant);

        authorizationProcessingServiceMock.Verify(
            expression: service =>
                service.EnsureUserIsPortalAdminWithPrivilege(
                    privilege: "tenant_create"),
            times: Times.Once);

        roleOrchestrationServiceMock.Verify(
expression: x => x.AddSSORoleAsync(item: It.Is<SSORole>(role =>
                role.Name == "Tenant Two Admins"
                && !role.UsersArePortalAdmins
                && role.Privs == "tenant_read,tenant_admin")),
times: Times.Once);

        userRoleOrchestrationServiceMock.Verify(
expression: x => x.AddSSOUserRoleAsync(userRole: It.Is<SSOUserRole>(userRole => userRole.UserId == "existing-admin")),
times: Times.Once);
    }
}