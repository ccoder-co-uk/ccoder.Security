// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal class TenantProcessingService(ITenantService tenantService)
        : ITenantProcessingService
{
    public ValueTask<Tenant> AddTenantAsync(Tenant newTenant) =>
        tenantService.AddTenantAsync(tenant: newTenant);

    public ValueTask DeleteTenantAsync(Tenant deletedTenant) =>
        tenantService.DeleteTenantAsync(tenant: deletedTenant);

    public IQueryable<Tenant> GetAllTenants() =>
        tenantService.GetAllTenants();

    public ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant) =>
        tenantService.UpdateTenantAsync(tenant: updatedTenant);
}