using cCoder.Security.Data.Brokers.DateTime;
using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class TenantService : ITenantService
{
    private readonly ITenantBroker broker;
    private readonly ISecurityDateTimeOffsetBroker dateTimeOffsetBroker;

    public TenantService(ITenantBroker broker, ISecurityDateTimeOffsetBroker dateTimeOffsetBroker)
    {
        this.broker = broker;
        this.dateTimeOffsetBroker = dateTimeOffsetBroker;
    }

    public async ValueTask<Tenant> AddTenantAsync(Tenant tenant)
    {
        tenant.LastUpdated = dateTimeOffsetBroker.GetCurrentTime();
        tenant.CreatedOn = dateTimeOffsetBroker.GetCurrentTime();
        return await broker.AddTenantAsync(tenant);
    }

    public async ValueTask DeleteTenantAsync(Tenant tenant)
        => await broker.DeleteTenantAsync(tenant);

    public IQueryable<Tenant> GetAllTenants()
        => broker.GetAllTenants();

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant tenant)
    {
        tenant.LastUpdated = dateTimeOffsetBroker.GetCurrentTime();
        return await broker.UpdateTenantAsync(tenant);
    }
}
