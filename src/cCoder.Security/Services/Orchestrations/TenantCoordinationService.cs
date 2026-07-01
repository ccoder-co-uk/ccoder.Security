using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;

namespace cCoder.Security.Services.Orchestrations;
internal class TenantCoordinationService(
    ITenantOrchestrationService tenantOrchestrationService,
    ITenantRelationsOrchestrationService tenantRelationsOrchestrationService,
    ISSOAuthorizationBroker authBroker)
        : ITenantCoordinationService
{
    public IQueryable<Tenant> GetAllTenants() =>
        tenantOrchestrationService.GetAllTenants();

    public async ValueTask<Tenant> AddTenantAsync(Tenant tenant) =>
        await tenantOrchestrationService.AddTenantAsync(tenant);

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant tenant) =>
        await tenantOrchestrationService.UpdateTenantAsync(tenant);

    public async ValueTask DeleteTenantAsync(Tenant tenant)
    {
        authBroker.UserIsPortalAdminWithPrivilege("tenant_delete");

        await tenantRelationsOrchestrationService.DeleteTenantRelationsAsync(tenant);
        await tenantOrchestrationService.DeleteTenantAsync(tenant);
    }
}