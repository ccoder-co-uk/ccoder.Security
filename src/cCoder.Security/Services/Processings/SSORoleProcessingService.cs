using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;
internal class SSORoleProcessingService(ISSORoleService ssoRoleService) 
    : ISSORoleProcessingService
{
    public IQueryable<SSORole> GetAllSSORoles(bool ignoreFilters = false) =>
        ssoRoleService.GetAllSSORoles(ignoreFilters);

    public async ValueTask<SSORole> AddSSORoleAsync(SSORole item) =>
        await ssoRoleService.AddSSORoleAsync(item);

    public async ValueTask DeleteSSORoleAsync(SSORole item) => 
        await ssoRoleService.DeleteSSORoleAsync(item);

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole item) =>
        await ssoRoleService.UpdateSSORoleAsync(item);
}


