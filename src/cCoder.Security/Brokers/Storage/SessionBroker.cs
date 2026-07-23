// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage;

internal class SessionBroker(ISecurityDbContextFactory contextFactory) : ISessionBroker
{
    public async ValueTask<Session> AddSessionAsync(Session Session)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Session> entityEntry = await context.Sessions.AddAsync(entity: Session);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<Session> UpdateSessionAsync(Session Session)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Session> entityEntry = context.Sessions.Update(entity: Session);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteSessionAsync(Session Session)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Session> entityEntry = context.Sessions.Remove(entity: Session);
        await context.SaveChangesAsync();
    }

    public IQueryable<Session> GetAllSessions()
    {
        SecurityDbContext context = contextFactory.CreateDbContext();
        return context.Sessions;
    }
}