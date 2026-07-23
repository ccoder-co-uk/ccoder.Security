// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

using Microsoft.EntityFrameworkCore;

namespace cCoder.Security.Brokers.Storage;

internal class SSORoleBroker(ISecurityDbContextFactory contextFactory)
    : ISSORoleBroker
{
    public async ValueTask<SSORole> InsertSSORoleAsync(SSORole role)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSORole> entityEntry = await context.Roles.AddAsync(entity: role);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole role)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSORole> entityEntry = context.Roles.Update(entity: role);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteSSORoleAsync(SSORole role)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSORole> entityEntry = context.Roles.Remove(entity: role);
        await context.SaveChangesAsync();
    }

    public IQueryable<SSORole> SelectAllSSORoles(bool ignoreFilters = false)
    {
        SecurityDbContext context = contextFactory.CreateDbContext();
        IQueryable<SSORole> roles = context.Roles;

        return ignoreFilters
            ? roles.IgnoreQueryFilters()
            : roles;
    }
}