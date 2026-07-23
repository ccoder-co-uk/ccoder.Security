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
    public async ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent)
    {
        userEvent.CreatedOn = dateTimeOffsetBroker.GetCurrentTime();
        UserEvent storageUserEvent = new()
        {
            Id = userEvent.Id,
            EventName = userEvent.EventName,
            Value = userEvent.Value,
            CreatedOn = userEvent.CreatedOn,
            SessionId = userEvent.SessionId,
            TenantId = userEvent.TenantId,
            CreatedBy = userEvent.CreatedBy
        };

        UserEvent result = await broker.AddUserEventAsync(userEvent: storageUserEvent);
        userEvent.Id = result.Id;
        userEvent.EventName = result.EventName;
        userEvent.Value = result.Value;
        userEvent.CreatedOn = result.CreatedOn;
        userEvent.SessionId = result.SessionId;
        userEvent.TenantId = result.TenantId;
        userEvent.CreatedBy = result.CreatedBy;
        return userEvent;
    }

    public async ValueTask DeleteUserEventAsync(UserEvent userEvent) =>
        await broker.DeleteUserEventAsync(userEvent: userEvent);

    public IQueryable<UserEvent> GetAllUserEvents() =>
        broker.GetAllUserEvents();
}