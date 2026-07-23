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
    public async ValueTask<UserEvent> InsertUserEventAsync(UserEvent newUserEvent)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<UserEvent> entityEntry =
            await context.UserEvents.AddAsync(entity: newUserEvent);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<UserEvent> UpdateUserEventAsync(UserEvent updatedUserEvent)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<UserEvent> entityEntry =
            context.UserEvents.Update(entity: updatedUserEvent);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteUserEventAsync(UserEvent deletedUserEvent)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<UserEvent> entityEntry =
            context.UserEvents.Remove(entity: deletedUserEvent);

        await context.SaveChangesAsync();
    }

    public IQueryable<UserEvent> SelectAllUserEvents()
    {
        SecurityDbContext context =
            contextFactory.CreateDbContext();

        return context.UserEvents;
    }
}