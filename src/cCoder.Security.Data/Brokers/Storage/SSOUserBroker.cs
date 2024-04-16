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
        using var context = contextFactory.CreateDbContext();
        return context.GetCurrentUser();
    }
    
    public IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false)
    {
        var context = contextFactory.CreateDbContext();

        return ignoreFilters
            ? context.Users.IgnoreQueryFilters()
            : context.Users;
    }

    public async ValueTask<SSOUser> AddSSOUserAsync(SSOUser user)
    {
        using var context = contextFactory.CreateDbContext();

        var entityEntry = await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser user)
    {
        using var context = contextFactory.CreateDbContext();

        var entityEntry = context.Users.Update(user);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteSSOUserAsync(SSOUser SSOUser)
    {
        using var context = contextFactory.CreateDbContext();

        var entityEntry = context.Users.Remove(SSOUser);
        await context.SaveChangesAsync();
    }
}