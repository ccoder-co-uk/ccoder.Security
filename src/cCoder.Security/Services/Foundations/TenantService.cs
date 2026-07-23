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
    public async ValueTask<Tenant> AddTenantAsync(Tenant tenant)
    {
        tenant.LastUpdated = DateTimeOffset.UtcNow;
        tenant.CreatedOn = tenant.LastUpdated;

        Tenant storageTenant = new()
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Description = tenant.Description,
            CreatedBy = tenant.CreatedBy,
            LastUpdatedBy = tenant.LastUpdatedBy,
            CreatedOn = tenant.CreatedOn,
            LastUpdated = tenant.LastUpdated
        };

        Tenant result = await broker.AddTenantAsync(tenant: storageTenant);
        tenant.Id = result.Id;
        tenant.Name = result.Name;
        tenant.Description = result.Description;
        tenant.CreatedBy = result.CreatedBy;
        tenant.LastUpdatedBy = result.LastUpdatedBy;
        tenant.CreatedOn = result.CreatedOn;
        tenant.LastUpdated = result.LastUpdated;
        return tenant;
    }

    public ValueTask DeleteTenantAsync(Tenant tenant)
        =>
        broker.DeleteTenantAsync(tenant: tenant);

    public IQueryable<Tenant> GetAllTenants()
        =>
        broker.GetAllTenants();

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant tenant)
    {
        tenant.LastUpdated = DateTimeOffset.UtcNow;

        Tenant storageTenant = new()
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Description = tenant.Description,
            CreatedBy = tenant.CreatedBy,
            LastUpdatedBy = tenant.LastUpdatedBy,
            CreatedOn = tenant.CreatedOn,
            LastUpdated = tenant.LastUpdated
        };

        Tenant result = await broker.UpdateTenantAsync(tenant: storageTenant);
        tenant.Id = result.Id;
        tenant.Name = result.Name;
        tenant.Description = result.Description;
        tenant.CreatedBy = result.CreatedBy;
        tenant.LastUpdatedBy = result.LastUpdatedBy;
        tenant.CreatedOn = result.CreatedOn;
        tenant.LastUpdated = result.LastUpdated;
        return tenant;
    }
}