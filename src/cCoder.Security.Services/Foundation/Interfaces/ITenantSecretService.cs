using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundation.Interfaces;

public interface ITenantSecretService
{
    ValueTask<TenantSecret> AddTenantSecretAsync(TenantSecret item);
    ValueTask<TenantSecret> UpdateTenantSecretAsync(TenantSecret item);
    ValueTask DeleteTenantSecretAsync(TenantSecret item);
    IQueryable<TenantSecret> GetAllTenantSecrets();
}
