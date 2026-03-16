using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestration.Interfaces;

public interface ITenantSecretOrchestrationService
{
    IQueryable<TenantSecret> GetAllTenantSecrets();
    
    string GetTenantSecretByKeyDecryptedAsync(string tenantId, string key);
    
    ValueTask<TenantSecret> AddTenantSecretAsync(TenantSecret item);
    
    ValueTask<TenantSecret> UpdateTenantSecretAsync(Guid id, TenantSecret item);
    
    ValueTask DeleteTenantSecretAsync(Guid key);
}
