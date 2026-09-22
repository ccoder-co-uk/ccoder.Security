// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;

namespace cCoder.Security.Exposures;

internal sealed class TenantAdministrationManager(
    ITenantAggregationService tenantAggregationService)
        : ITenantAdministrationManager
{
    public ValueTask<Tenant> AddTenantAsync(Tenant newTenant) =>
        tenantAggregationService.AddTenantAsync(item: newTenant);

    public ValueTask DeleteTenantAsync(Tenant deletedTenant) =>
        tenantAggregationService.DeleteTenantAsync(item: deletedTenant);

    public IQueryable<Tenant> GetAllTenants() =>
        tenantAggregationService.GetAllTenants();

    public ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant) =>
        tenantAggregationService.UpdateTenantAsync(item: updatedTenant);
}