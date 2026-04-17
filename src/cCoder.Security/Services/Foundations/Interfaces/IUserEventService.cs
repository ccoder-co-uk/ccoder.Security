using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;
internal interface IUserEventService
{
    ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent);
    ValueTask DeleteUserEventAsync(UserEvent userEvent);
    IQueryable<UserEvent> GetAllUserEvents();
}


