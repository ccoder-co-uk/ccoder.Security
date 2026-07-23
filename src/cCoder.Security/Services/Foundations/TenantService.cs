// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class TenantService(ITenantBroker broker)
    : ITenantService
{
    public ValueTask<Tenant> AddTenantAsync(Tenant newTenant) =>
        TryCatch<Tenant>(operation: async () =>
        {
            ValidateTenantOnAdd(newTenant: newTenant);
            newTenant.LastUpdated = DateTimeOffset.UtcNow;
            newTenant.CreatedOn = newTenant.LastUpdated;

            Tenant storageTenant = CreateStorageTenant(tenant: newTenant);
            Tenant result = await broker.InsertTenantAsync(tenant: storageTenant);
            CopyTenant(sourceTenant: result, targetTenant: newTenant);

            return newTenant;
        });

    public ValueTask DeleteTenantAsync(Tenant deletedTenant) =>
        TryCatch(operation: async () =>
        {
            ValidateTenantOnDelete(deletedTenant: deletedTenant);

            await broker.DeleteTenantAsync(tenant: deletedTenant);
        });

    public IQueryable<Tenant> GetAllTenants() =>
        TryCatch(operation: () => broker.SelectAllTenants());

    public ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant) =>
        TryCatch<Tenant>(operation: async () =>
        {
            ValidateTenantOnUpdate(updatedTenant: updatedTenant);
            updatedTenant.LastUpdated = DateTimeOffset.UtcNow;

            Tenant storageTenant = CreateStorageTenant(tenant: updatedTenant);
            Tenant result = await broker.UpdateTenantAsync(tenant: storageTenant);
            CopyTenant(sourceTenant: result, targetTenant: updatedTenant);

            return updatedTenant;
        });

    private static Tenant CreateStorageTenant(Tenant tenant) =>
        new()
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Description = tenant.Description,
            CreatedBy = tenant.CreatedBy,
            LastUpdatedBy = tenant.LastUpdatedBy,
            CreatedOn = tenant.CreatedOn,
            LastUpdated = tenant.LastUpdated
        };

    private static void CopyTenant(Tenant sourceTenant, Tenant targetTenant)
    {
        targetTenant.Id = sourceTenant.Id;
        targetTenant.Name = sourceTenant.Name;
        targetTenant.Description = sourceTenant.Description;
        targetTenant.CreatedBy = sourceTenant.CreatedBy;
        targetTenant.LastUpdatedBy = sourceTenant.LastUpdatedBy;
        targetTenant.CreatedOn = sourceTenant.CreatedOn;
        targetTenant.LastUpdated = sourceTenant.LastUpdated;
    }
}