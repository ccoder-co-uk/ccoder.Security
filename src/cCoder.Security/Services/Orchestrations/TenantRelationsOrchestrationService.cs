// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Orchestrations;

internal sealed partial class TenantRelationsOrchestrationService(
    ISSORoleProcessingService roleProcessingService,
    ISSOUserRoleProcessingService userRoleProcessingService,
    ITenantAnalysisProcessingService tenantAnalysisProcessingService)
        : ITenantRelationsOrchestrationService
{
    public ValueTask DeleteTenantRelationsAsync(Tenant deletedTenant) =>
        TryCatch(operation: async () =>
        {
            ValidateTenantRelationsOnDelete(deletedTenant: deletedTenant);

            SSORole[] tenantRoles = roleProcessingService
                .GetAllSSORoles()
                .Where(predicate: role => role.TenantId == deletedTenant.Id)
                .ToArray();

            SSOUserRole[] userRoles = userRoleProcessingService
                .GetAllSSOUserRoles()
                .Where(predicate: userRole => tenantRoles
                    .Select(selector: tenantRole => tenantRole.Id)
                    .Contains(value: userRole.RoleId))
                .ToArray();

            TenantAnalysis[] tenantAnalysis = tenantAnalysisProcessingService
                .GetAllTenantAnalysis()
                .Where(predicate: analysis =>
                    analysis.TenantId == deletedTenant.Id)
                .ToArray();

            foreach (TenantAnalysis analysis in tenantAnalysis)
            {
                await tenantAnalysisProcessingService
                    .DeleteTenantAnalysisAsync(item: analysis);
            }

            foreach (SSOUserRole userRole in userRoles)
            {
                await userRoleProcessingService.DeleteSSOUserRoleAsync(
                    item: userRole);
            }

            foreach (SSORole role in tenantRoles)
            {
                await roleProcessingService.DeleteSSORoleAsync(item: role);
            }
        });
}