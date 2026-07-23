// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class TenantProcessingService(ITenantService tenantService)
        : ITenantProcessingService
{
    public ValueTask<Tenant> AddTenantAsync(Tenant newTenant) =>
        TryCatch<Tenant>(operation: async () =>
        {
            ValidateTenantOnAdd(newTenant: newTenant);

            return await tenantService.AddTenantAsync(tenant: newTenant);
        });

    public ValueTask DeleteTenantAsync(Tenant deletedTenant) =>
        TryCatch(operation: async () =>
        {
            ValidateTenantOnDelete(deletedTenant: deletedTenant);

            await tenantService.DeleteTenantAsync(tenant: deletedTenant);
        });

    public IQueryable<Tenant> GetAllTenants() =>
        TryCatch(operation: () =>
        {
            ValidateTenantsOnGet();

            return tenantService.GetAllTenants();
        });

    public ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant) =>
        TryCatch<Tenant>(operation: async () =>
        {
            ValidateTenantOnUpdate(updatedTenant: updatedTenant);

            return await tenantService.UpdateTenantAsync(tenant: updatedTenant);
        });
}