// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Orchestrations;

internal class TenantOrchestrationService(
    ITenantProcessingService tenantProcessingService,
    ISSOUserProcessingService userProcessingService,
    ISSORoleOrchestrationService roleOrchestrationService,
    ISSOUserRoleOrchestrationService userRoleOrchestrationService,
    ISSOAuthorizationBroker authBroker)
        : ITenantOrchestrationService
{
    public IQueryable<Tenant> GetAllTenants()
    {
        authBroker.UserHasPrivilege(privilege: "tenant_read");

        return tenantProcessingService.GetAllTenants();
    }

    public async ValueTask<Tenant> AddTenantAsync(Tenant newTenant)
    {
        bool isFirstTenant = !tenantProcessingService
            .GetAllTenants()
            .Any();

        if (!isFirstTenant)
        { authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_create"); }

        var existing = tenantProcessingService
            .GetAllTenants()
            .FirstOrDefault(predicate: t => t.Id == newTenant.Id);

        if (existing != null)
        { throw new ValidationException($"Tenant '{newTenant.Id}' already exists."); }

        var dbTenant = await tenantProcessingService.AddTenantAsync(item: newTenant);

        string bootstrapUserId = ResolveBootstrapUserId(tenant: newTenant, isFirstTenant: isFirstTenant);

        string[] rolePrivileges = isFirstTenant
            ? [.. authBroker
                .GetAllPrivileges()
                .Select(selector: privilege => privilege.Id)]
            : ["tenant_read", "tenant_admin"];

        var role = await roleOrchestrationService.AddSSORoleAsync(item: new SSORole()
        {
            UsersArePortalAdmins = isFirstTenant,
            Name = isFirstTenant ? "Administrators" : $"{newTenant.Name} Admins",
            Description = isFirstTenant ? "Bootstrap tenant administrators" : $"{newTenant.Name} Admins",
            Privs = string.Join(separator: ',', value: rolePrivileges),
            TenantId = newTenant.Id
        });

        if (BootstrapUserExists(userId: bootstrapUserId))
        {
            await userRoleOrchestrationService.AddSSOUserRoleAsync(userRole: new SSOUserRole()
            {
                UserId = bootstrapUserId,
                RoleId = role.Id
            });
        }

        return dbTenant;
    }

    public async ValueTask DeleteTenantAsync(Tenant deletedTenant)
    {
        authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_delete");

        await tenantProcessingService.DeleteTenantAsync(item: deletedTenant);
    }

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant)
    {
        authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_update");

        return await tenantProcessingService.UpdateTenantAsync(item: updatedTenant);
    }

    private string ResolveBootstrapUserId(Tenant tenant, bool isFirstTenant)
    {
        string currentUserId = authBroker.GetCurrentUser()?.Id;

        if (!string.IsNullOrWhiteSpace(value: currentUserId) &&
            !string.Equals(a: currentUserId, b: "Guest", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return currentUserId;
        }

        if (!string.IsNullOrWhiteSpace(value: tenant?.CreatedBy))
        { return tenant.CreatedBy; }

        if (isFirstTenant)
        { throw new ValidationException("CreatedBy is required when bootstrapping the first tenant."); }

        return null;
    }

    private bool BootstrapUserExists(string userId) =>
        !string.IsNullOrWhiteSpace(value: userId)
        && userProcessingService.FindById(ssoUserId: userId) is not null;
}