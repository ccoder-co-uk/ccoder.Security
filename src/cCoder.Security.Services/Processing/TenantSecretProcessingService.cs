using cCoder.Security.Data;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Processing;

public class TenantSecretProcessingService(
    ITenantSecretService tenantSecretService,
    ISymmetricCrypto<string> crypto)
    : ITenantSecretProcessingService
{

    public IQueryable<TenantSecret> GetAllTenantSecrets() =>
        tenantSecretService.GetAllTenantSecrets()
            .Select(tenantSecret => new TenantSecret
            {
                Id = tenantSecret.Id,
                TenantId = tenantSecret.TenantId,
                Key = tenantSecret.Key,
                CreatedOn = tenantSecret.CreatedOn,
                CreatedBy = tenantSecret.CreatedBy,
                UpdatedOn = tenantSecret.UpdatedOn,
                UpdatedBy = tenantSecret.UpdatedBy
            });

    public string GetDecryptedTenantSecretByKeyAsync(string tenantId, string key)
    {
        TenantSecret tenantSecret = tenantSecretService
            .GetAllTenantSecrets()
            .FirstOrDefault(secret => secret.TenantId == tenantId && secret.Key == key);

        if (tenantSecret is null)
            return null;

        return crypto.Decrypt(tenantSecret.Value);
    }
    
    public async ValueTask<TenantSecret> AddTenantSecretAsync(TenantSecret tenantSecret)
    {
        tenantSecret.Value = crypto.Encrypt(tenantSecret.Value);

        return await tenantSecretService.AddTenantSecretAsync(tenantSecret);
    }

    public async ValueTask<TenantSecret> UpdateTenantSecretAsync(TenantSecret tenantSecret, bool encrypt = true)
    {
        if (tenantSecret.Value != null && encrypt)
            tenantSecret.Value = crypto.Encrypt(tenantSecret.Value);

        return await tenantSecretService.UpdateTenantSecretAsync(tenantSecret);
    }

    public async ValueTask DeleteTenantSecretAsync(TenantSecret tenantSecret) =>
        await tenantSecretService.DeleteTenantSecretAsync(tenantSecret);
}
