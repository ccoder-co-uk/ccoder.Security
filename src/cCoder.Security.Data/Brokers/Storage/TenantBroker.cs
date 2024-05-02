using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage;

public class TenantBroker : ITenantBroker
{
    private readonly ISecurityDbContextFactory contextFactory;

    public TenantBroker(ISecurityDbContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async ValueTask<Tenant> AddTenantAsync(Tenant tenant)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Tenant> entityEntry = await context.Tenants.AddAsync(tenant);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant tenant)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Tenant> entityEntry = context.Tenants.Update(tenant);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteTenantAsync(Tenant tenant)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Tenant> entityEntry = context.Tenants.Remove(tenant);
        await context.SaveChangesAsync();
    }

    public IQueryable<Tenant> GetAllTenants()
    {
        EF.SecurityDbContext context = contextFactory.CreateDbContext();
        return context.Tenants;
    }
}