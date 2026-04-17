using System.ComponentModel.DataAnnotations;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Orchestration;

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
            .Setup(x => x.GetAllTenants())
            .Returns(Array.Empty<Tenant>().AsQueryable());
        tenantProcessingServiceMock
            .Setup(x => x.AddTenantAsync(inputTenant))
            .Returns(new ValueTask<Tenant>(inputTenant));
        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new SSOUser { Id = "Guest" });
        authorizationBrokerMock
            .Setup(x => x.GetAllPrivileges())
            .Returns(
                new[]
                {
                    new SSOPrivilege { Id = "tenant_read" },
                    new SSOPrivilege { Id = "tenant_admin" },
                    new SSOPrivilege { Id = "tenant_create" }
                });
        roleOrchestrationServiceMock
            .Setup(x => x.AddSSORoleAsync(It.IsAny<SSORole>()))
            .ReturnsAsync((SSORole role) => role);
        userProcessingServiceMock
            .Setup(x => x.FindById("setup-admin"))
            .Returns((SSOUser)null);

        Tenant result = await tenantOrchestrationService.AddTenantAsync(inputTenant);

        result.Should().BeSameAs(inputTenant);
        roleOrchestrationServiceMock.Verify(
            x => x.AddSSORoleAsync(It.Is<SSORole>(role =>
                role.Name == "Administrators"
                && role.UsersArePortalAdmins
                && role.TenantId == inputTenant.Id
                && role.Privs.Contains("tenant_create"))),
            Times.Once);
        userRoleOrchestrationServiceMock.Verify(
            x => x.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()),
            Times.Never);
        authorizationBrokerMock.Verify(x => x.UserIsPortalAdminWithPrivilege(It.IsAny<string>()), Times.Never);
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
            .Setup(x => x.GetAllTenants())
            .Returns(Array.Empty<Tenant>().AsQueryable());
        tenantProcessingServiceMock
            .Setup(x => x.AddTenantAsync(inputTenant))
            .Returns(new ValueTask<Tenant>(inputTenant));
        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new SSOUser { Id = "Guest" });
        authorizationBrokerMock
            .Setup(x => x.GetAllPrivileges())
            .Returns(
                new[]
                {
                    new SSOPrivilege { Id = "tenant_read" },
                    new SSOPrivilege { Id = "tenant_admin" },
                    new SSOPrivilege { Id = "tenant_create" }
                });
        roleOrchestrationServiceMock
            .Setup(x => x.AddSSORoleAsync(It.IsAny<SSORole>()))
            .ReturnsAsync((SSORole role) => role);
        userProcessingServiceMock
            .Setup(x => x.FindById("setup-admin"))
            .Returns(new SSOUser { Id = "setup-admin" });
        userRoleOrchestrationServiceMock
            .Setup(x => x.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()))
            .ReturnsAsync((SSOUserRole userRole) => userRole);

        await tenantOrchestrationService.AddTenantAsync(inputTenant);

        userRoleOrchestrationServiceMock.Verify(
            x => x.AddSSOUserRoleAsync(It.Is<SSOUserRole>(userRole =>
                userRole.UserId == "setup-admin")),
            Times.Once);
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
            .Setup(x => x.GetAllTenants())
            .Returns(Array.Empty<Tenant>().AsQueryable());
        tenantProcessingServiceMock
            .Setup(x => x.AddTenantAsync(inputTenant))
            .Returns(new ValueTask<Tenant>(inputTenant));
        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new SSOUser { Id = "Guest" });

        Func<Task> act = async () => await tenantOrchestrationService.AddTenantAsync(inputTenant);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("CreatedBy is required when bootstrapping the first tenant.");
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
            .Setup(x => x.GetAllTenants())
            .Returns(new[] { new Tenant { Id = "tenant-1", Name = "Existing" } }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.UserIsPortalAdminWithPrivilege("tenant_create"));
        tenantProcessingServiceMock
            .Setup(x => x.AddTenantAsync(inputTenant))
            .Returns(new ValueTask<Tenant>(inputTenant));
        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new SSOUser { Id = "existing-admin" });
        roleOrchestrationServiceMock
            .Setup(x => x.AddSSORoleAsync(It.IsAny<SSORole>()))
            .ReturnsAsync((SSORole role) => role);
        userProcessingServiceMock
            .Setup(x => x.FindById("existing-admin"))
            .Returns(new SSOUser { Id = "existing-admin" });
        userRoleOrchestrationServiceMock
            .Setup(x => x.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()))
            .ReturnsAsync((SSOUserRole userRole) => userRole);

        await tenantOrchestrationService.AddTenantAsync(inputTenant);

        authorizationBrokerMock.Verify(x => x.UserIsPortalAdminWithPrivilege("tenant_create"), Times.Once);
        roleOrchestrationServiceMock.Verify(
            x => x.AddSSORoleAsync(It.Is<SSORole>(role =>
                role.Name == "Tenant Two Admins"
                && !role.UsersArePortalAdmins
                && role.Privs == "tenant_read,tenant_admin")),
            Times.Once);
        userRoleOrchestrationServiceMock.Verify(
            x => x.AddSSOUserRoleAsync(It.Is<SSOUserRole>(userRole => userRole.UserId == "existing-admin")),
            Times.Once);
    }
}
