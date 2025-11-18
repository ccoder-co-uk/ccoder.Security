using cCoder.Security.Data.Brokers.DateTime;
using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class UserEventService(IUserEventBroker broker, ISecurityDateTimeOffsetBroker dateTimeOffsetBroker) 
    : IUserEventService
{
    public async ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent)
    {
        userEvent.CreatedOn = dateTimeOffsetBroker.GetCurrentTime();
        return await broker.AddUserEventAsync(userEvent);
    }

    public async ValueTask DeleteUserEventAsync(UserEvent userEvent) => 
        await broker.DeleteUserEventAsync(userEvent);

    public IQueryable<UserEvent> GetAllUserEvents() => 
        broker.GetAllUserEvents();
}
