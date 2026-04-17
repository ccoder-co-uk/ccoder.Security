using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;
internal class SSOUserRoleProcessingService(ISSOUserRoleService ssoUserRoleService)
    : ISSOUserRoleProcessingService
{
    public IQueryable<SSOUserRole> GetAllSSOUserRoles() =>
        ssoUserRoleService.GetAllSSOUserRoles();

    public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole item) =>
        await ssoUserRoleService.AddSSOUserRoleAsync(item);

    public async ValueTask DeleteSSOUserRoleAsync(SSOUserRole item) => 
        await ssoUserRoleService.DeleteSSOUserRoleAsync(item);
}


