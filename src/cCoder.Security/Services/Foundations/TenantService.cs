// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal class TenantService(ITenantBroker broker)
    : ITenantService
{
    public async ValueTask<Tenant> AddTenantAsync(Tenant newTenant)
    {
        newTenant.LastUpdated = DateTimeOffset.UtcNow;
        newTenant.CreatedOn = newTenant.LastUpdated;

        Tenant storageTenant = new()
        {
            Id = newTenant.Id,
            Name = newTenant.Name,
            Description = newTenant.Description,
            CreatedBy = newTenant.CreatedBy,
            LastUpdatedBy = newTenant.LastUpdatedBy,
            CreatedOn = newTenant.CreatedOn,
            LastUpdated = newTenant.LastUpdated
        };

        Tenant result = await broker.InsertTenantAsync(tenant: storageTenant);
        newTenant.Id = result.Id;
        newTenant.Name = result.Name;
        newTenant.Description = result.Description;
        newTenant.CreatedBy = result.CreatedBy;
        newTenant.LastUpdatedBy = result.LastUpdatedBy;
        newTenant.CreatedOn = result.CreatedOn;
        newTenant.LastUpdated = result.LastUpdated;
        return newTenant;
    }

    public ValueTask DeleteTenantAsync(Tenant deletedTenant)
        =>
        broker.DeleteTenantAsync(tenant: deletedTenant);

    public IQueryable<Tenant> GetAllTenants()
        =>
        broker.SelectAllTenants();

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant)
    {
        updatedTenant.LastUpdated = DateTimeOffset.UtcNow;

        Tenant storageTenant = new()
        {
            Id = updatedTenant.Id,
            Name = updatedTenant.Name,
            Description = updatedTenant.Description,
            CreatedBy = updatedTenant.CreatedBy,
            LastUpdatedBy = updatedTenant.LastUpdatedBy,
            CreatedOn = updatedTenant.CreatedOn,
            LastUpdated = updatedTenant.LastUpdated
        };

        Tenant result = await broker.UpdateTenantAsync(tenant: storageTenant);
        updatedTenant.Id = result.Id;
        updatedTenant.Name = result.Name;
        updatedTenant.Description = result.Description;
        updatedTenant.CreatedBy = result.CreatedBy;
        updatedTenant.LastUpdatedBy = result.LastUpdatedBy;
        updatedTenant.CreatedOn = result.CreatedOn;
        updatedTenant.LastUpdated = result.LastUpdated;
        return updatedTenant;
    }
}