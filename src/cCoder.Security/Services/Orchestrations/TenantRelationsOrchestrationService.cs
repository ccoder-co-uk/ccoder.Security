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
    public async ValueTask DeleteTenantRelationsAsync(Tenant deletedTenant)
    {
        authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_delete");

        var tenantRoles = roleProcessingService
            .GetAllSSORoles()
            .Where(predicate: r => r.TenantId == deletedTenant.Id)
            .ToArray();

        var userRoles = userRoleProcessingService
            .GetAllSSOUserRoles()
            .Where(predicate: ur => tenantRoles
                .Select(selector: tr => tr.Id)
                .Contains(value: ur.RoleId))
            .ToArray();

        var tenantAnalysis = tenantAnalysisProcessingService
            .GetAllTenantAnalysis()
            .Where(predicate: ta => ta.TenantId == deletedTenant.Id)
            .ToArray();

        foreach (var analysis in tenantAnalysis)
        { await tenantAnalysisProcessingService.DeleteTenantAnalysisAsync(deletedTenantAnalysis: analysis); }

        foreach (var userRole in userRoles)
        { await userRoleProcessingService.DeleteSSOUserRoleAsync(deletedSSOUserRole: userRole); }

        foreach (var role in tenantRoles)
        { await roleProcessingService.DeleteSSORoleAsync(deletedSSORole: role); }
    }
}