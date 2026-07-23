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

    public ValueTask<SSORole> AddSSORoleAsync(SSORole newSSORole) =>
        ssoRoleService.AddSSORoleAsync(item: newSSORole);

    public ValueTask DeleteSSORoleAsync(SSORole deletedSSORole) =>
        ssoRoleService.DeleteSSORoleAsync(item: deletedSSORole);

    public ValueTask<SSORole> UpdateSSORoleAsync(SSORole updatedSSORole) =>
        ssoRoleService.UpdateSSORoleAsync(item: updatedSSORole);
}