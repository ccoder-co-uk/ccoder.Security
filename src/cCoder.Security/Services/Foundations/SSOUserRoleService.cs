using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;
internal class SSOUserRoleService(
    ISSOUserRoleBroker userRoleBroker) 
        : ISSOUserRoleService
{
    public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole item) =>
        await userRoleBroker.AddSSOUserRoleAsync(item);

    public async ValueTask DeleteSSOUserRoleAsync(SSOUserRole item) =>
        await userRoleBroker.DeleteSSOUserRoleAsync(item);

    public IQueryable<SSOUserRole> GetAllSSOUserRoles()  =>
        userRoleBroker.GetAllSSOUserRoles();
}


