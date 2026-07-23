// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Orchestrations;

internal sealed partial class SSORoleOrchestrationService(
    ISSORoleProcessingService roleProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : ISSORoleOrchestrationService
{
    public IQueryable<SSORole> GetAllSSORoles() =>
        TryCatch(operation: () =>
        {
            ValidateSSORolesOnGet();

            return roleProcessingService.GetAllSSORoles();
        });

    public ValueTask<SSORole> AddSSORoleAsync(SSORole newSSORole) =>
        TryCatch<SSORole>(operation: async () =>
        {
            ValidateSSORoleOnAdd(newSSORole: newSSORole);

            if (roleProcessingService
                .GetAllSSORoles()
                .Any())
            {
                authorizationProcessingService
                    .EnsureUserIsPortalAdminWithPrivilege(
                        privilege: "tenant_admin");
            }

            return await roleProcessingService.AddSSORoleAsync(
                item: newSSORole);
        });

    public ValueTask<SSORole> UpdateSSORoleAsync(SSORole updatedSSORole) =>
        TryCatch<SSORole>(operation: async () =>
        {
            ValidateSSORoleOnUpdate(updatedSSORole: updatedSSORole);

            authorizationProcessingService
                .EnsureUserIsPortalAdminWithPrivilege(
                    privilege: "tenant_admin");

            return await roleProcessingService.UpdateSSORoleAsync(
                item: updatedSSORole);
        });

    public ValueTask DeleteSSORoleAsync(SSORole deletedSSORole) =>
        TryCatch(operation: async () =>
        {
            ValidateSSORoleOnDelete(deletedSSORole: deletedSSORole);

            authorizationProcessingService
                .EnsureUserIsPortalAdminWithPrivilege(
                    privilege: "tenant_admin");

            await roleProcessingService.DeleteSSORoleAsync(
                item: deletedSSORole);
        });
}