// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Events;

internal interface ITenantDeletingCleanupHandler
{
    ValueTask HandleAsync(string tenantId);
}

internal sealed class TenantDeletingCleanupHandler(
    ISSORoleProcessingService roleProcessingService,
    ISSOUserRoleProcessingService userRoleProcessingService,
    ITenantAnalysisProcessingService tenantAnalysisProcessingService)
    : ITenantDeletingCleanupHandler
{
    public async ValueTask HandleAsync(string tenantId)
    {
        SSORole[] tenantRoles = roleProcessingService
            .GetAllSSORoles()
            .Where(predicate: role => role.TenantId == tenantId)
            .ToArray();

        SSOUserRole[] userRoles = userRoleProcessingService
            .GetAllSSOUserRoles()
            .Where(predicate: userRole => tenantRoles
                .Select(selector: tenantRole => tenantRole.Id)
                .Contains(value: userRole.RoleId))
            .ToArray();

        TenantAnalysis[] tenantAnalysis = tenantAnalysisProcessingService
            .GetAllTenantAnalyses()
            .Where(predicate: analysis =>
                analysis.TenantId == tenantId)
            .ToArray();

        foreach (TenantAnalysis analysis in tenantAnalysis)
        {
            await tenantAnalysisProcessingService.DeleteTenantAnalysisAsync(
                item: analysis);
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
    }
}