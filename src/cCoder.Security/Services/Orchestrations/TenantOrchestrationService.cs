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
        authBroker.UserHasPrivilege("tenant_read");

        return tenantProcessingService.GetAllTenants();
    }

    public async ValueTask<Tenant> AddTenantAsync(Tenant tenant)
    {
        bool isFirstTenant = !tenantProcessingService.GetAllTenants().Any();

        if (!isFirstTenant)
            authBroker.UserIsPortalAdminWithPrivilege("tenant_create");

        var existing = tenantProcessingService
            .GetAllTenants()
            .FirstOrDefault(t => t.Id == tenant.Id);

        if (existing != null)
            throw new ValidationException($"Tenant '{tenant.Id}' already exists.");

        var dbTenant = await tenantProcessingService.AddTenantAsync(tenant);

        string bootstrapUserId = ResolveBootstrapUserId(tenant, isFirstTenant);
        string[] rolePrivileges = isFirstTenant
            ? [.. authBroker.GetAllPrivileges().Select(privilege => privilege.Id)]
            : ["tenant_read", "tenant_admin"];

        var role = await roleOrchestrationService.AddSSORoleAsync(new SSORole()
        {
            UsersArePortalAdmins = isFirstTenant,
            Name = isFirstTenant ? "Administrators" : $"{tenant.Name} Admins",
            Description = isFirstTenant ? "Bootstrap tenant administrators" : $"{tenant.Name} Admins",
            Privs = string.Join(',', rolePrivileges),
            TenantId = tenant.Id
        });

        if (BootstrapUserExists(bootstrapUserId))
        {
            await userRoleOrchestrationService.AddSSOUserRoleAsync(new SSOUserRole()
            {
                UserId = bootstrapUserId,
                RoleId = role.Id
            });
        }

        return dbTenant;
    }

    public async ValueTask DeleteTenantAsync(Tenant tenant)
    {
        authBroker.UserIsPortalAdminWithPrivilege("tenant_delete");

        await tenantProcessingService.DeleteTenantAsync(tenant);
    }
    
    public async ValueTask<Tenant> UpdateTenantAsync(Tenant tenant)
    {
        authBroker.UserIsPortalAdminWithPrivilege("tenant_update");

        return await tenantProcessingService.UpdateTenantAsync(tenant);
    }

    private string ResolveBootstrapUserId(Tenant tenant, bool isFirstTenant)
    {
        string currentUserId = authBroker.GetCurrentUser()?.Id;

        if (!string.IsNullOrWhiteSpace(currentUserId) &&
            !string.Equals(currentUserId, "Guest", StringComparison.OrdinalIgnoreCase))
        {
            return currentUserId;
        }

        if (!string.IsNullOrWhiteSpace(tenant?.CreatedBy))
            return tenant.CreatedBy;

        if (isFirstTenant)
            throw new ValidationException("CreatedBy is required when bootstrapping the first tenant.");

        return null;
    }

    private bool BootstrapUserExists(string userId) =>
        !string.IsNullOrWhiteSpace(userId) && userProcessingService.FindById(userId) is not null;
}



