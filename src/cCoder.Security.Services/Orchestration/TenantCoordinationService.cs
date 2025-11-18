using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;

namespace cCoder.Security.Services.Orchestration;

public class TenantCoordinationService(
    ITenantOrchestrationService tenantOrchestrationService,
    ITenantRelationsOrchestrationService tenantRelationsOrchestrationService,
    ISSOAuthorizationBroker authBroker)
        : ITenantCoordinationService
{
    public async ValueTask DeleteTenantAsync(Tenant tenant)
    {
        authBroker.UserIsPortalAdminWithPrivilege("tenant_delete");

        await tenantRelationsOrchestrationService.DeleteTenantRelationsAsync(tenant);
        await tenantOrchestrationService.DeleteTenantAsync(tenant);
    }
}