using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage;

public class SSOUserRoleBroker(ISecurityDbContextFactory contextFactory) 
    : ISSOUserRoleBroker
{
    public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole userRole)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSOUserRole> entityEntry = await context.UserRoles.AddAsync(userRole);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteSSOUserRoleAsync(SSOUserRole userRole)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SSOUserRole> entityEntry = context.UserRoles.Remove(userRole);
        await context.SaveChangesAsync();
    }

    public IQueryable<SSOUserRole> GetAllSSOUserRoles()
    {
        EF.SecurityDbContext context = contextFactory.CreateDbContext();
        return context.UserRoles;
    }
}