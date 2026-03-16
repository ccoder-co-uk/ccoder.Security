using System.ComponentModel.DataAnnotations;
using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Orchestration;

public class TenantSecretOrchestrationService(
    ITenantSecretProcessingService tenantSecretProcessingService,
    ISSOAuthorizationBroker authBroker)
    : ITenantSecretOrchestrationService
{
    public IQueryable<TenantSecret> GetAllTenantSecrets()
    {
        SSOUser currentUser = authBroker.GetCurrentUser();

        string[] tenantIds = currentUser.Roles
            .Where(role => role.Role.TenantId != null && role.Role.Privs.Contains("tenantsecret_read"))
            .Select(role => role.Role.TenantId)
            .Distinct()
            .ToArray();

        return tenantSecretProcessingService
            .GetAllTenantSecrets()
            .Where(secret => tenantIds.Contains(secret.TenantId));
    }

    public string GetTenantSecretByKeyDecryptedAsync(string tenantId, string key)
    {
        authBroker.UserHasPrivilege("tenantsecret_read", tenantId);

        return tenantSecretProcessingService.GetDecryptedTenantSecretByKeyAsync(tenantId, key);
    }

    public async ValueTask<TenantSecret> AddTenantSecretAsync(TenantSecret tenantSecret)
    {
        authBroker.UserHasPrivilege("tenantsecret_create", tenantSecret.TenantId);

        ValidateTenantSecretIsUnique(tenantSecret.TenantId, tenantSecret.Key);

        TenantSecret created = await tenantSecretProcessingService.AddTenantSecretAsync(tenantSecret);

        return created;
    }

    public async ValueTask<TenantSecret> UpdateTenantSecretAsync(Guid id, TenantSecret secret)
    {
        TenantSecret existing = tenantSecretProcessingService
            .GetAllTenantSecrets()
            .FirstOrDefault(s => s.Id == id);

        if (existing is null)
            return null;

        authBroker.UserHasPrivilege("tenantsecret_update", secret.TenantId);

        if (secret.Value != null)
            existing.Value = secret.Value;

        TenantSecret updated = await tenantSecretProcessingService.UpdateTenantSecretAsync(existing);

        return updated;
    }

    public async ValueTask DeleteTenantSecretAsync(Guid key)
    {
        TenantSecret existingTenantSecret = tenantSecretProcessingService
            .GetAllTenantSecrets()
            .FirstOrDefault(secret => secret.Id == key);

        if (existingTenantSecret is null)
            return;

        authBroker.UserHasPrivilege("tenantsecret_delete", existingTenantSecret.TenantId);

        await tenantSecretProcessingService.DeleteTenantSecretAsync(existingTenantSecret);
    }

    private void ValidateTenantSecretIsUnique(string tenantId, string key)
    {
        bool alreadyExists = tenantSecretProcessingService
            .GetAllTenantSecrets()
            .Any(secret => secret.TenantId == tenantId && secret.Key == key);

        if (alreadyExists)
            throw new ValidationException($"Tenant Secret '{key}' already exists for tenant '{tenantId}'.");
    }
}
