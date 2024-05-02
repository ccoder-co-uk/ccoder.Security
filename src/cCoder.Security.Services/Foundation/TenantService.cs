using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class TenantService(ITenantBroker broker) 
    : ITenantService
{
    public async ValueTask<Tenant> AddTenantAsync(Tenant tenant)
    {
        tenant.LastUpdated = DateTimeOffset.UtcNow;
        tenant.CreatedOn = tenant.LastUpdated;
        return await broker.AddTenantAsync(tenant);
    }

    public async ValueTask DeleteTenantAsync(Tenant tenant)
        => await broker.DeleteTenantAsync(tenant);

    public IQueryable<Tenant> GetAllTenants()
        => broker.GetAllTenants();

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant tenant)
    {
        tenant.LastUpdated = DateTimeOffset.UtcNow;
        return await broker.UpdateTenantAsync(tenant);
    }
}
