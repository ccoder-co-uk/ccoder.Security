using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage.Interfaces;

public interface IUserEventBroker
{
    ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent);
    ValueTask DeleteUserEventAsync(UserEvent userEvent);
    IQueryable<UserEvent> GetAllUserEvents();
    ValueTask<UserEvent> UpdateUserEventAsync(UserEvent userEvent);
}