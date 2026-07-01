using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;
public interface IUserEventProcessingService
{
    ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent);

    ValueTask DeleteUserEventAsync(UserEvent userEvent);

    IQueryable<UserEvent> GetAllUserEvents();
}