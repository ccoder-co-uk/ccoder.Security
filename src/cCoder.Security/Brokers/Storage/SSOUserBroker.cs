// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Security.Brokers.Storage;

internal class SSOUserBroker(ISecurityDbContextFactory contextFactory)
    : ISSOUserBroker
{
    public SSOUser SelectCurrentSSOUser()
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();
        return context.GetCurrentUser();
    }

    public IQueryable<SSOUser> SelectAllSSOUsers(bool ignoreFilters = false)
    {
        SecurityDbContext context = contextFactory.CreateDbContext(ignoreAuthInfo: ignoreFilters);

        return ignoreFilters
            ? context.Users.IgnoreQueryFilters()
            : context.Users;
    }

    public async ValueTask<SSOUser> InsertSSOUserAsync(SSOUser newSSOUser)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSOUser> entityEntry = await context.Users.AddAsync(entity: newSSOUser);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser updatedSSOUser)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSOUser> entityEntry = context.Users.Update(entity: updatedSSOUser);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSOUser> entityEntry = context.Users.Remove(entity: deletedSSOUser);
        await context.SaveChangesAsync();
    }
}