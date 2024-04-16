using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage;

public class UserEventBroker : IUserEventBroker
{
    ISecurityDbContextFactory contextFactory;

    public UserEventBroker(ISecurityDbContextFactory contextFactory)
        => this.contextFactory = contextFactory;

    public async ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent)
    {
        using var context = contextFactory.CreateDbContext();

        var entityEntry = await context.UserEvents.AddAsync(userEvent);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<UserEvent> UpdateUserEventAsync(UserEvent userEvent)
    {
        using var context = contextFactory.CreateDbContext();

        var entityEntry = context.UserEvents.Update(userEvent);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteUserEventAsync(UserEvent userEvent)
    {
        using var context = contextFactory.CreateDbContext();

        var entityEntry = context.UserEvents.Remove(userEvent);
        await context.SaveChangesAsync();
    }

    public IQueryable<UserEvent> GetAllUserEvents()
    {
        var context = contextFactory.CreateDbContext();
        return context.UserEvents;
    }
}