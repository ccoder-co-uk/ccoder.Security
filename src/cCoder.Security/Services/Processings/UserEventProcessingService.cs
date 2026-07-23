// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal class UserEventProcessingService(IUserEventService userEventService)
        : IUserEventProcessingService
{
    public ValueTask<UserEvent> AddUserEventAsync(UserEvent newUserEvent) =>
        userEventService.AddUserEventAsync(newUserEvent: newUserEvent);

    public ValueTask DeleteUserEventAsync(UserEvent deletedUserEvent) =>
        userEventService.DeleteUserEventAsync(deletedUserEvent: deletedUserEvent);

    public IQueryable<UserEvent> GetAllUserEvents() =>
        userEventService.GetAllUserEvents();
}