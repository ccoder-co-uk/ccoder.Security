// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

    public ValueTask<Tenant> AddTenantAsync(Tenant newTenant) =>
        tenantOrchestrationService.AddTenantAsync(item: newTenant);

    public ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant) =>
        tenantOrchestrationService.UpdateTenantAsync(item: updatedTenant);

    public async ValueTask DeleteTenantAsync(Tenant deletedTenant)
    {
        authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_delete");

        await tenantRelationsOrchestrationService.DeleteTenantRelationsAsync(item: deletedTenant);
        await tenantOrchestrationService.DeleteTenantAsync(item: deletedTenant);
    }
}