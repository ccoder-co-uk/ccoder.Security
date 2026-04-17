using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;
internal class UserEventProcessingService(IUserEventService userEventService)
        : IUserEventProcessingService
{
    public ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent) => 
        userEventService.AddUserEventAsync(userEvent);

    public ValueTask DeleteUserEventAsync(UserEvent userEvent) => 
        userEventService.DeleteUserEventAsync(userEvent);

    public IQueryable<UserEvent> GetAllUserEvents() => 
        userEventService.GetAllUserEvents();
}


