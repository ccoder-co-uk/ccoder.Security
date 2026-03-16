using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace cCoder.Security.Data.Brokers.Storage;

public class TenantSecretBroker(ISecurityDbContextFactory contextFactory)
    : ITenantSecretBroker
{
    public IQueryable<TenantSecret> GetAllTenantSecrets()
    {
        var context = contextFactory.CreateDbContext();
        return context.TenantSecrets;
    }
    
    public async ValueTask<TenantSecret> AddTenantSecretAsync(TenantSecret tenantSecret)
    {
        using var context = contextFactory.CreateDbContext();

        EntityEntry<TenantSecret> entityEntry = await context.TenantSecrets.AddAsync(tenantSecret);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<TenantSecret> UpdateTenantSecretAsync(TenantSecret tenantSecret)
    {
        using var context = contextFactory.CreateDbContext();

        EntityEntry<TenantSecret> entityEntry = context.TenantSecrets.Update(tenantSecret);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteTenantSecretAsync(TenantSecret tenantSecret)
    {
        using var context = contextFactory.CreateDbContext();

        context.TenantSecrets.Remove(tenantSecret);

        await context.SaveChangesAsync();
    }
}
