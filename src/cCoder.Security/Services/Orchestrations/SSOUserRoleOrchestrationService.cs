// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Orchestrations;

internal sealed partial class SSOUserRoleOrchestrationService(
    ISSOUserRoleProcessingService userRoleProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : ISSOUserRoleOrchestrationService
{
    public IQueryable<SSOUserRole> GetAllSSOUserRoles() =>
        TryCatch(operation: () =>
        {
            ValidateSSOUserRolesOnGet();

            authorizationProcessingService.EnsureUserHasPrivilege(
                privilege: "userrole_read");

            return userRoleProcessingService.GetAllSSOUserRoles();
        });

    public ValueTask<SSOUserRole> AddSSOUserRoleAsync(
        SSOUserRole newSSOUserRole) =>
        TryCatch<SSOUserRole>(operation: async () =>
        {
            ValidateSSOUserRoleOnAdd(newSSOUserRole: newSSOUserRole);

            if (userRoleProcessingService
                .GetAllSSOUserRoles()
                .Any())
            {
                authorizationProcessingService
                    .EnsureUserIsPortalAdminWithPrivilege(
                        privilege: "userrole_create");
            }

            return await userRoleProcessingService.AddSSOUserRoleAsync(
                item: newSSOUserRole);
        });

    public ValueTask DeleteSSOUserRoleAsync(
        SSOUserRole deletedSSOUserRole) =>
        TryCatch(operation: async () =>
        {
            ValidateSSOUserRoleOnDelete(
                deletedSSOUserRole: deletedSSOUserRole);

            authorizationProcessingService
                .EnsureUserIsPortalAdminWithPrivilege(
                    privilege: "userrole_delete");

            await userRoleProcessingService.DeleteSSOUserRoleAsync(
                item: deletedSSOUserRole);
        });
}