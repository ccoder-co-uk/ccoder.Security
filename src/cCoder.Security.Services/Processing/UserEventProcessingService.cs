using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Processing;

public class UserEventProcessingService 
    : IUserEventProcessingService
{
    private readonly IUserEventService userEventService;

    public UserEventProcessingService(IUserEventService userEventService) =>
        this.userEventService = userEventService;

    public ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent) => 
        userEventService.AddUserEventAsync(userEvent);

    public ValueTask DeleteUserEventAsync(UserEvent userEvent) => 
        userEventService.DeleteUserEventAsync(userEvent);

    public IQueryable<UserEvent> GetAllUserEvents() => 
        userEventService.GetAllUserEvents();
}
