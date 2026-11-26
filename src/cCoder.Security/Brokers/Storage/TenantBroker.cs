// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Models.Entities;
using Microsoft.EntityFrameworkCore;
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

        Tenant tenant = await context.Tenants
            .IgnoreQueryFilters()
            .SingleAsync(predicate: item => item.Id == deletedTenant.Id);

        IQueryable<SSORole> tenantRoles = context.Roles
            .IgnoreQueryFilters()
            .Where(predicate: role => role.TenantId == tenant.Id);

        Guid[] tenantRoleIds = await tenantRoles
            .Select(selector: role => role.Id)
            .ToArrayAsync();

        SSOUserRole[] userRoles = await context.UserRoles
            .IgnoreQueryFilters()
            .Where(predicate: role => tenantRoleIds.Contains(role.RoleId))
            .ToArrayAsync();

        TenantAnalysis[] analyses = await context.TenantAnalysis
            .IgnoreQueryFilters()
            .Where(predicate: analysis => analysis.TenantId == tenant.Id)
            .ToArrayAsync();

        context.RemoveRange(entities: userRoles);
        context.RemoveRange(entities: analyses);
        context.RemoveRange(entities: await tenantRoles.ToArrayAsync());
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
