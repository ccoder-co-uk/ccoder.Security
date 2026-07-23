// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class SSOUserRoleProcessingService(ISSOUserRoleService ssoUserRoleService)
    : ISSOUserRoleProcessingService
{
    public IQueryable<SSOUserRole> GetAllSSOUserRoles() =>
        TryCatch(operation: () =>
        {
            ValidateSSOUserRolesOnGet();

            return ssoUserRoleService.GetAllSSOUserRoles();
        });

    public ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole newSSOUserRole) =>
        TryCatch<SSOUserRole>(operation: async () =>
        {
            ValidateSSOUserRoleOnAdd(newSSOUserRole: newSSOUserRole);

            return await ssoUserRoleService.AddSSOUserRoleAsync(item: newSSOUserRole);
        });

    public ValueTask DeleteSSOUserRoleAsync(SSOUserRole deletedSSOUserRole) =>
        TryCatch(operation: async () =>
        {
            ValidateSSOUserRoleOnDelete(deletedSSOUserRole: deletedSSOUserRole);

            await ssoUserRoleService.DeleteSSOUserRoleAsync(item: deletedSSOUserRole);
        });
}