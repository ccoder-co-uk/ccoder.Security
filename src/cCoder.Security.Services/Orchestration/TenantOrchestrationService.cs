using System.ComponentModel.DataAnnotations;
using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Orchestration;

public class TenantOrchestrationService(
    ITenantProcessingService tenantProcessingService,
    ISSORoleProcessingService roleProcessingService,
    ISSOUserRoleProcessingService userRoleProcessingService,
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
        authBroker.UserIsPortalAdminWithPrivilege("tenant_create");

        var existing = tenantProcessingService
            .GetAllTenants()
            .FirstOrDefault(t => t.Id == tenant.Id);

        if (existing != null)
            throw new ValidationException($"Tenant '{tenant.Id}' already exists.");

        var dbTenant = await tenantProcessingService.AddTenantAsync(tenant);

        var role = await roleProcessingService.AddSSORoleAsync(new SSORole()
        {
            UsersArePortalAdmins = false,
            Name = $"{tenant.Name} Admins",
            Description = $"{tenant.Name} Admins",
            Privs = "tenant_read",
            TenantId = tenant.Id
        });

        var user = authBroker.GetCurrentUser();

        await userRoleProcessingService.AddSSOUserRoleAsync(new SSOUserRole()
        {
            UserId = user.Id,
            RoleId = role.Id
        });

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
}