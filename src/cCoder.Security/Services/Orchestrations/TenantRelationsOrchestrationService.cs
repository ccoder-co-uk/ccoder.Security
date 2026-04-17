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
        authBroker.UserIsPortalAdminWithPrivilege("tenant_delete");

        var tenantRoles = roleProcessingService
            .GetAllSSORoles()
            .Where(r => r.TenantId == tenant.Id)
            .ToArray();

        var userRoles = userRoleProcessingService
            .GetAllSSOUserRoles()
            .Where(ur => tenantRoles.Select(tr => tr.Id).Contains(ur.RoleId))
            .ToArray();

        var tenantAnalysis = tenantAnalysisProcessingService
            .GetAllTenantAnalysis()
            .Where(ta => ta.TenantId == tenant.Id)
            .ToArray();

        foreach (var analysis in tenantAnalysis)
            await tenantAnalysisProcessingService.DeleteTenantAnalysisAsync(analysis);

        foreach (var userRole in userRoles)
            await userRoleProcessingService.DeleteSSOUserRoleAsync(userRole);

        foreach (var role in tenantRoles)
            await roleProcessingService.DeleteSSORoleAsync(role);
    }
}


