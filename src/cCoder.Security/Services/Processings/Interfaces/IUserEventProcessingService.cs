using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;
internal interface IUserEventProcessingService
{
    ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent);
    ValueTask DeleteUserEventAsync(UserEvent userEvent);
    IQueryable<UserEvent> GetAllUserEvents();
}


