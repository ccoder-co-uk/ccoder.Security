// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class SSORoleProcessingService(ISSORoleService ssoRoleService)
    : ISSORoleProcessingService
{
    public IQueryable<SSORole> GetAllSSORoles(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateSSORolesOnGet(ignoreFilters: ignoreFilters);

            return ssoRoleService.GetAllSSORoles(ignoreFilters: ignoreFilters);
        });

    public ValueTask<SSORole> AddSSORoleAsync(SSORole newSSORole) =>
        TryCatch<SSORole>(operation: async () =>
        {
            ValidateSSORoleOnAdd(newSSORole: newSSORole);

            return await ssoRoleService.AddSSORoleAsync(item: newSSORole);
        });

    public ValueTask DeleteSSORoleAsync(SSORole deletedSSORole) =>
        TryCatch(operation: async () =>
        {
            ValidateSSORoleOnDelete(deletedSSORole: deletedSSORole);

            await ssoRoleService.DeleteSSORoleAsync(item: deletedSSORole);
        });

    public ValueTask<SSORole> UpdateSSORoleAsync(SSORole updatedSSORole) =>
        TryCatch<SSORole>(operation: async () =>
        {
            ValidateSSORoleOnUpdate(updatedSSORole: updatedSSORole);

            return await ssoRoleService.UpdateSSORoleAsync(item: updatedSSORole);
        });
}