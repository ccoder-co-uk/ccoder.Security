// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.DateTime;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal class UserEventService(IUserEventBroker broker, ISecurityDateTimeOffsetBroker dateTimeOffsetBroker)
    : IUserEventService
{
    public async ValueTask<UserEvent> AddUserEventAsync(UserEvent newUserEvent)
    {
        newUserEvent.CreatedOn = dateTimeOffsetBroker.GetCurrentTime();

        UserEvent storageUserEvent = new()
        {
            Id = newUserEvent.Id,
            EventName = newUserEvent.EventName,
            Value = newUserEvent.Value,
            CreatedOn = newUserEvent.CreatedOn,
            SessionId = newUserEvent.SessionId,
            TenantId = newUserEvent.TenantId,
            CreatedBy = newUserEvent.CreatedBy
        };

        UserEvent result = await broker.InsertUserEventAsync(userEvent: storageUserEvent);
        newUserEvent.Id = result.Id;
        newUserEvent.EventName = result.EventName;
        newUserEvent.Value = result.Value;
        newUserEvent.CreatedOn = result.CreatedOn;
        newUserEvent.SessionId = result.SessionId;
        newUserEvent.TenantId = result.TenantId;
        newUserEvent.CreatedBy = result.CreatedBy;
        return newUserEvent;
    }

    public ValueTask DeleteUserEventAsync(UserEvent deletedUserEvent) =>
        broker.DeleteUserEventAsync(userEvent: deletedUserEvent);

    public IQueryable<UserEvent> GetAllUserEvents() =>
        broker.SelectAllUserEvents();
}