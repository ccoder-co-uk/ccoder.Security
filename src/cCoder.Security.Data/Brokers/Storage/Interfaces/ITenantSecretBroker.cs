using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage.Interfaces;

public interface ITenantSecretBroker
{
    ValueTask<TenantSecret> AddTenantSecretAsync(TenantSecret tenantSecret);
    ValueTask<TenantSecret> UpdateTenantSecretAsync(TenantSecret tenantSecret);
    ValueTask DeleteTenantSecretAsync(TenantSecret tenantSecret);
    IQueryable<TenantSecret> GetAllTenantSecrets();
}
