using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Security.Data.Brokers.Storage;

public class SSOUserBroker(ISecurityDbContextFactory contextFactory) 
    : ISSOUserBroker
{
    public SSOUser Me()
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();
        return context.GetCurrentUser();
    }
    
    public IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false)
    {
        EF.SecurityDbContext context = contextFactory.CreateDbContext();

        return ignoreFilters
            ? context.Users.IgnoreQueryFilters()
            : context.Users;
    }

    public async ValueTask<SSOUser> AddSSOUserAsync(SSOUser user)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSOUser> entityEntry = await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser user)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSOUser> entityEntry = context.Users.Update(user);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteSSOUserAsync(SSOUser SSOUser)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSOUser> entityEntry = context.Users.Remove(SSOUser);
        await context.SaveChangesAsync();
    }
}