// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class UserEventProcessingService(IUserEventService userEventService)
        : IUserEventProcessingService
{
    public ValueTask<UserEvent> AddUserEventAsync(UserEvent newUserEvent) =>
        TryCatch<UserEvent>(operation: async () =>
        {
            ValidateUserEventOnAdd(newUserEvent: newUserEvent);

            return await userEventService.AddUserEventAsync(userEvent: newUserEvent);
        });

    public ValueTask DeleteUserEventAsync(UserEvent deletedUserEvent) =>
        TryCatch(operation: async () =>
        {
            ValidateUserEventOnDelete(deletedUserEvent: deletedUserEvent);

            await userEventService.DeleteUserEventAsync(userEvent: deletedUserEvent);
        });

    public IQueryable<UserEvent> GetAllUserEvents() =>
        TryCatch(operation: () =>
        {
            ValidateUserEventsOnGet();

            return userEventService.GetAllUserEvents();
        });
}