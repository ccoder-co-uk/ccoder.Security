// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;
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
                    .EnsureUserHasPrivilege(
                        privilege: "tenant_admin",
                        tenantId: newSSORole.TenantId);
            }

            return await roleProcessingService.AddSSORoleAsync(
                item: newSSORole);
        });

    public ValueTask<SSORole> UpdateSSORoleAsync(SSORole updatedSSORole) =>
        TryCatch<SSORole>(operation: async () =>
        {
            ValidateSSORoleOnUpdate(updatedSSORole: updatedSSORole);

            authorizationProcessingService
                .EnsureUserHasPrivilege(
                    privilege: "tenant_admin",
                    tenantId: updatedSSORole.TenantId);

            return await roleProcessingService.UpdateSSORoleAsync(
                item: updatedSSORole);
        });

    public ValueTask DeleteSSORoleAsync(SSORole deletedSSORole) =>
        TryCatch(operation: async () =>
        {
            ValidateSSORoleOnDelete(deletedSSORole: deletedSSORole);

            authorizationProcessingService
                .EnsureUserHasPrivilege(
                    privilege: "tenant_admin",
                    tenantId: deletedSSORole.TenantId);

            await roleProcessingService.DeleteSSORoleAsync(
                item: deletedSSORole);
        });
}