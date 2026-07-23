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
    public async ValueTask<Tenant> InsertTenantAsync(Tenant newTenant)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        EntityEntry<Tenant> entityEntry =
            await context.Tenants.AddAsync(entity: newTenant);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<Tenant> entityEntry =
            context.Tenants.Update(entity: updatedTenant);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteTenantAsync(Tenant deletedTenant)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<Tenant> entityEntry =
            context.Tenants.Remove(entity: deletedTenant);

        await context.SaveChangesAsync();
    }

    public IQueryable<Tenant> SelectAllTenants()
    {
        SecurityDbContext context =
            contextFactory.CreateDbContext();

        return context.Tenants;
    }
}