// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.DateTime;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class UserEventService(
    IUserEventBroker broker,
    ISecurityDateTimeOffsetBroker dateTimeOffsetBroker)
    : IUserEventService
{
    public ValueTask<UserEvent> AddUserEventAsync(UserEvent newUserEvent) =>
        TryCatch<UserEvent>(operation: async () =>
        {
            ValidateUserEventOnAdd(newUserEvent: newUserEvent);
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

            UserEvent result = await broker.InsertUserEventAsync(
                userEvent: storageUserEvent);

            CopyUserEvent(sourceUserEvent: result, targetUserEvent: newUserEvent);

            return newUserEvent;
        });

    public ValueTask DeleteUserEventAsync(UserEvent deletedUserEvent) =>
        TryCatch(operation: async () =>
        {
            ValidateUserEventOnDelete(deletedUserEvent: deletedUserEvent);

            await broker.DeleteUserEventAsync(userEvent: deletedUserEvent);
        });

    public IQueryable<UserEvent> GetAllUserEvents() =>
        TryCatch(operation: () => broker.SelectAllUserEvents());

    private static void CopyUserEvent(
        UserEvent sourceUserEvent,
        UserEvent targetUserEvent)
    {
        targetUserEvent.Id = sourceUserEvent.Id;
        targetUserEvent.EventName = sourceUserEvent.EventName;
        targetUserEvent.Value = sourceUserEvent.Value;
        targetUserEvent.CreatedOn = sourceUserEvent.CreatedOn;
        targetUserEvent.SessionId = sourceUserEvent.SessionId;
        targetUserEvent.TenantId = sourceUserEvent.TenantId;
        targetUserEvent.CreatedBy = sourceUserEvent.CreatedBy;
    }
}