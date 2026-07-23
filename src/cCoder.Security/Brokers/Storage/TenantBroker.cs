// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace cCoder.Security.Brokers.Storage;

internal class TenantBroker(ISecurityDbContextFactory contextFactory)
    : ITenantBroker
{
    public async ValueTask<Tenant> InsertTenantAsync(Tenant tenant)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        EntityEntry<Tenant> entityEntry =
            await context.Tenants.AddAsync(entity: tenant);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant tenant)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<Tenant> entityEntry =
            context.Tenants.Update(entity: tenant);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteTenantAsync(Tenant tenant)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<Tenant> entityEntry =
            context.Tenants.Remove(entity: tenant);

        await context.SaveChangesAsync();
    }

    public IQueryable<Tenant> SelectAllTenants()
    {
        SecurityDbContext context =
            contextFactory.CreateDbContext();

        return context.Tenants;
    }
}