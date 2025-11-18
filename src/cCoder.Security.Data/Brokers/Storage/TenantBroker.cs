using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace cCoder.Security.Data.Brokers.Storage;

public class TenantBroker(ISecurityDbContextFactory contextFactory) 
    : ITenantBroker
{
    public async ValueTask<Tenant> AddTenantAsync(Tenant tenant)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        EntityEntry<Tenant> entityEntry = 
            await context.Tenants.AddAsync(tenant);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant tenant)
    {
        using EF.SecurityDbContext context = 
            contextFactory.CreateDbContext();

        EntityEntry<Tenant> entityEntry = 
            context.Tenants.Update(tenant);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteTenantAsync(Tenant tenant)
    {
        using EF.SecurityDbContext context = 
            contextFactory.CreateDbContext();

        EntityEntry<Tenant> entityEntry = 
            context.Tenants.Remove(tenant);

        await context.SaveChangesAsync();
    }

    public IQueryable<Tenant> GetAllTenants()
    {
        EF.SecurityDbContext context = 
            contextFactory.CreateDbContext();

        return context.Tenants;
    }
}