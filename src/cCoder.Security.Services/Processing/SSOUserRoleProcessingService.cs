using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Processing;

public class SSOUserRoleProcessingService(ISSOUserRoleService ssoUserRoleService)
    : ISSOUserRoleProcessingService
{
    public IQueryable<SSOUserRole> GetAllSSOUserRoles() =>
        ssoUserRoleService.GetAllSSOUserRoles();

    public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole item) =>
        await ssoUserRoleService.AddSSOUserRoleAsync(item);

    public async ValueTask DeleteSSOUserRoleAsync(SSOUserRole item) => 
        await ssoUserRoleService.DeleteSSOUserRoleAsync(item);
}