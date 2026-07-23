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
    public ValueTask<Tenant> AddTenantAsync(Tenant item) =>
        tenantService.AddTenantAsync(tenant: item);

    public ValueTask DeleteTenantAsync(Tenant item) =>
        tenantService.DeleteTenantAsync(tenant: item);

    public IQueryable<Tenant> GetAllTenants() =>
        tenantService.GetAllTenants();

    public ValueTask<Tenant> UpdateTenantAsync(Tenant item) =>
        tenantService.UpdateTenantAsync(tenant: item);
}