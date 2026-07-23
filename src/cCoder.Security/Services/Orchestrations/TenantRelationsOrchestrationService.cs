// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Orchestrations;

internal class TenantRelationsOrchestrationService(
    ISSORoleProcessingService roleProcessingService,
    ISSOUserRoleProcessingService userRoleProcessingService,
    ITenantAnalysisProcessingService tenantAnalysisProcessingService,
    ISSOAuthorizationBroker authBroker)
        : ITenantRelationsOrchestrationService
{
    public async ValueTask DeleteTenantRelationsAsync(Tenant tenant)
    {
        authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_delete");

        var tenantRoles = roleProcessingService
            .GetAllSSORoles()
            .Where(predicate: r => r.TenantId == tenant.Id)
            .ToArray();

        var userRoles = userRoleProcessingService
            .GetAllSSOUserRoles()
            .Where(predicate: ur => tenantRoles.Select(tr => tr.Id).Contains(ur.RoleId))
            .ToArray();

        var tenantAnalysis = tenantAnalysisProcessingService
            .GetAllTenantAnalysis()
            .Where(predicate: ta => ta.TenantId == tenant.Id)
            .ToArray();

        foreach (var analysis in tenantAnalysis)
        { await tenantAnalysisProcessingService.DeleteTenantAnalysisAsync(item: analysis); }

        foreach (var userRole in userRoles)
        { await userRoleProcessingService.DeleteSSOUserRoleAsync(item: userRole); }

        foreach (var role in tenantRoles)
        { await roleProcessingService.DeleteSSORoleAsync(item: role); }
    }
}