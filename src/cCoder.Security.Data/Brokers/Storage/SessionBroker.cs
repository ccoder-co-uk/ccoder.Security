using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage;

public class SessionBroker : ISessionBroker
{
    private readonly ISecurityDbContextFactory contextFactory;

    public SessionBroker(ISecurityDbContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async ValueTask<Session> AddSessionAsync(Session Session)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Session> entityEntry = await context.Sessions.AddAsync(Session);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<Session> UpdateSessionAsync(Session Session)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Session> entityEntry = context.Sessions.Update(Session);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteSessionAsync(Session Session)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Session> entityEntry = context.Sessions.Remove(Session);
        await context.SaveChangesAsync();
    }

    public IQueryable<Session> GetAllSessions()
    {
        EF.SecurityDbContext context = contextFactory.CreateDbContext();
        return context.Sessions;
    }
}
