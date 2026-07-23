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

    public ValueTask<SSORole> AddSSORoleAsync(SSORole item) =>
        ssoRoleService.AddSSORoleAsync(item: item);

    public ValueTask DeleteSSORoleAsync(SSORole item) =>
        ssoRoleService.DeleteSSORoleAsync(item: item);

    public ValueTask<SSORole> UpdateSSORoleAsync(SSORole item) =>
        ssoRoleService.UpdateSSORoleAsync(item: item);
}