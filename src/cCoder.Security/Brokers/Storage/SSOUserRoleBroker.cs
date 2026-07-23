// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage;

internal class SSOUserRoleBroker(ISecurityDbContextFactory contextFactory)
    : ISSOUserRoleBroker
{
    public async ValueTask<SSOUserRole> InsertSSOUserRoleAsync(SSOUserRole newSSOUserRole)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSOUserRole> entityEntry = await context.UserRoles.AddAsync(entity: newSSOUserRole);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteSSOUserRoleAsync(SSOUserRole deletedSSOUserRole)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSOUserRole> entityEntry = context.UserRoles.Remove(entity: deletedSSOUserRole);
        await context.SaveChangesAsync();
    }

    public IQueryable<SSOUserRole> SelectAllSSOUserRoles()
    {
        SecurityDbContext context = contextFactory.CreateDbContext();
        return context.UserRoles;
    }
}