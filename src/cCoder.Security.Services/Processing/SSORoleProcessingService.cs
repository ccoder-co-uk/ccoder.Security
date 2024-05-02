using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Processing;

public class SSORoleProcessingService 
    : ISSORoleProcessingService
{
    private readonly ISSORoleService ssoRoleService;

    public SSORoleProcessingService(ISSORoleService ssoRoleService)
    {
        this.ssoRoleService = ssoRoleService;
    }

    public async ValueTask<SSORole> AddSSORoleAsync(SSORole item) =>
        await ssoRoleService.AddSSORoleAsync(item);

    public async ValueTask DeleteSSORoleAsync(SSORole item) => await 
        ssoRoleService.DeleteSSORoleAsync(item);

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole item) =>
        await ssoRoleService.UpdateSSORoleAsync(item);
}