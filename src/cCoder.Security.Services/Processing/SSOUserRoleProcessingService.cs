using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Processing;

public class SSOUserRoleProcessingService 
    : ISSOUserRoleProcessingService
{
    private readonly ISSOUserRoleService ssoUserRoleService;

    public SSOUserRoleProcessingService(ISSOUserRoleService ssoUserRoleService)
    {
        this.ssoUserRoleService = ssoUserRoleService;
    }

    public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole item) => 
        await ssoUserRoleService.AddSSOUserRoleAsync(item);

    public async ValueTask DeleteSSOUserRoleAsync(SSOUserRole item) => 
        await ssoUserRoleService.DeleteSSOUserRoleAsync(item);
}
