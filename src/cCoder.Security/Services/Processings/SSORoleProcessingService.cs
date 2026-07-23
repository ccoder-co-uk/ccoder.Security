// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal class SSORoleProcessingService(ISSORoleService ssoRoleService)
    : ISSORoleProcessingService
{
    public IQueryable<SSORole> GetAllSSORoles(bool ignoreFilters = false) =>
        ssoRoleService.GetAllSSORoles(ignoreFilters: ignoreFilters);

    public async ValueTask<SSORole> AddSSORoleAsync(SSORole item) =>
        await ssoRoleService.AddSSORoleAsync(item: item);

    public async ValueTask DeleteSSORoleAsync(SSORole item) =>
        await ssoRoleService.DeleteSSORoleAsync(item: item);

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole item) =>
        await ssoRoleService.UpdateSSORoleAsync(item: item);
}