using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class TenantSecretService(ITenantSecretBroker broker)
    : ITenantSecretService
{
    public IQueryable<TenantSecret> GetAllTenantSecrets() =>
        broker.GetAllTenantSecrets();
    
    public async ValueTask<TenantSecret> AddTenantSecretAsync(TenantSecret tenantSecret)
    {
        tenantSecret.UpdatedOn = DateTimeOffset.UtcNow;
        tenantSecret.CreatedOn = tenantSecret.UpdatedOn;

        return await broker.AddTenantSecretAsync(tenantSecret);
    }

    public async ValueTask<TenantSecret> UpdateTenantSecretAsync(TenantSecret tenantSecret)
    {
        tenantSecret.UpdatedOn = DateTimeOffset.UtcNow;

        return await broker.UpdateTenantSecretAsync(tenantSecret);
    }

    public async ValueTask DeleteTenantSecretAsync(TenantSecret tenantSecret) =>
        await broker.DeleteTenantSecretAsync(tenantSecret);

}
