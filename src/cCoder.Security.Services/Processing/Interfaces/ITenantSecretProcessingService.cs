using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processing.Interfaces;

public interface ITenantSecretProcessingService
{
    ValueTask<TenantSecret> AddTenantSecretAsync(TenantSecret item);
    
    ValueTask<TenantSecret> UpdateTenantSecretAsync(TenantSecret item, bool encrypt = true);
    
    ValueTask DeleteTenantSecretAsync(TenantSecret item);
    
    IQueryable<TenantSecret> GetAllTenantSecrets();
    
    string GetDecryptedTenantSecretByKeyAsync(string tenantId, string key);
}