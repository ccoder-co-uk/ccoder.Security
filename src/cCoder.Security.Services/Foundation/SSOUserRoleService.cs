using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class SSOUserRoleService(
    ISSOUserRoleBroker userRoleBroker,
    ITenantBroker tenantBroker,
    ISSOAuthorizationBroker authBroker) 
        : ISSOUserRoleService
{
    public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole item) =>
        await userRoleBroker.AddSSOUserRoleAsync(item);

    public async ValueTask DeleteSSOUserRoleAsync(SSOUserRole item) =>
        await userRoleBroker.DeleteSSOUserRoleAsync(item);

    public IQueryable<SSOUserRole> GetAllSSOUserRoles()
        => userRoleBroker.GetAllSSOUserRoles();
}