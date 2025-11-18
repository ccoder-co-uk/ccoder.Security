using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage;

public class SSORoleBroker(ISecurityDbContextFactory contextFactory) 
    : ISSORoleBroker
{
    public async ValueTask<SSORole> AddSSORoleAsync(SSORole role)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSORole> entityEntry = await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole role)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSORole> entityEntry = context.Roles.Update(role);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteSSORoleAsync(SSORole role)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSORole> entityEntry = context.Roles.Remove(role);
        await context.SaveChangesAsync();
    }

    public IQueryable<SSORole> GetAllSSORoles()
    {
        EF.SecurityDbContext context = contextFactory.CreateDbContext();
        return context.Roles;
    }
}