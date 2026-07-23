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
    public async ValueTask<SSORole> InsertSSORoleAsync(SSORole newSSORole)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSORole> entityEntry = await context.Roles.AddAsync(entity: newSSORole);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole updatedSSORole)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSORole> entityEntry = context.Roles.Update(entity: updatedSSORole);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteSSORoleAsync(SSORole deletedSSORole)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSORole> entityEntry = context.Roles.Remove(entity: deletedSSORole);
        await context.SaveChangesAsync();
    }

    public IQueryable<SSORole> SelectAllSSORoles() =>
        contextFactory
            .CreateDbContext()
            .Roles;

    public IQueryable<SSORole> SelectAllSSORolesIgnoringFilters() =>
        contextFactory
            .CreateDbContext(ignoreAuthInfo: true)
            .Roles
            .IgnoreQueryFilters();
}