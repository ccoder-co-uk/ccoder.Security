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

    public async ValueTask<Tenant> AddTenantAsync(Tenant tenant) =>
        await tenantOrchestrationService.AddTenantAsync(item: tenant);

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant tenant) =>
        await tenantOrchestrationService.UpdateTenantAsync(item: tenant);

    public async ValueTask DeleteTenantAsync(Tenant tenant)
    {
        authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_delete");

        await tenantRelationsOrchestrationService.DeleteTenantRelationsAsync(item: tenant);
        await tenantOrchestrationService.DeleteTenantAsync(item: tenant);
    }
}