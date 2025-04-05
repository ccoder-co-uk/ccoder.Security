using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class SSOUserRoleService(
    ISSOUserRoleBroker userRoleBroker,
    ISSOAuthorizationBroker authBroker) 
        : ISSOUserRoleService
{
    public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole item)
    {
        var tenantId = userRoleBroker.GetAllTenants()
            .First(t => t.Roles.Any(r => r.Id == item.RoleId))
            .Id;

        authBroker.UserHasPrivilege("tenant_admin", tenantId);
        await userRoleBroker.AddSSOUserRoleAsync(item);
    }

    public async ValueTask DeleteSSOUserRoleAsync(SSOUserRole item)
    {
        var tenantId = userRoleBroker.GetAllTenants()
            .First(t => t.Roles.Any(r => r.Id == item.RoleId))
            .Id;

        authBroker.UserHasPrivilege("tenant_admin", tenantId);
        await userRoleBroker.DeleteSSOUserRoleAsync(item);
    }

    public IQueryable<SSOUserRole> GetAllSSOUserRoles()
        => userRoleBroker.GetAllSSOUserRoles();
}