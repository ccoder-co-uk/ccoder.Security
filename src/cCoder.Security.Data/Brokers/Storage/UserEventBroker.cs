using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage;

public class UserEventBroker : IUserEventBroker
{
    private readonly ISecurityDbContextFactory contextFactory;

    public UserEventBroker(ISecurityDbContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<UserEvent> entityEntry = await context.UserEvents.AddAsync(userEvent);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<UserEvent> UpdateUserEventAsync(UserEvent userEvent)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<UserEvent> entityEntry = context.UserEvents.Update(userEvent);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteUserEventAsync(UserEvent userEvent)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<UserEvent> entityEntry = context.UserEvents.Remove(userEvent);
        await context.SaveChangesAsync();
    }

    public IQueryable<UserEvent> GetAllUserEvents()
    {
        EF.SecurityDbContext context = contextFactory.CreateDbContext();
        return context.UserEvents;
    }
}