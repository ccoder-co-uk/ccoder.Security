// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace cCoder.Security.Brokers.Storage;

internal class UserEventBroker(ISecurityDbContextFactory contextFactory)
    : IUserEventBroker
{
    public async ValueTask<UserEvent> InsertUserEventAsync(UserEvent userEvent)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<UserEvent> entityEntry =
            await context.UserEvents.AddAsync(entity: userEvent);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<UserEvent> UpdateUserEventAsync(UserEvent userEvent)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<UserEvent> entityEntry =
            context.UserEvents.Update(entity: userEvent);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteUserEventAsync(UserEvent userEvent)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<UserEvent> entityEntry =
            context.UserEvents.Remove(entity: userEvent);

        await context.SaveChangesAsync();
    }

    public IQueryable<UserEvent> SelectAllUserEvents()
    {
        SecurityDbContext context =
            contextFactory.CreateDbContext();

        return context.UserEvents;
    }
}