// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Exposures;

internal sealed class UserEventManager(
    IUserEventProcessingService userEventProcessingService)
        : IUserEventManager
{
    public ValueTask<UserEvent> AddUserEventAsync(UserEvent newUserEvent) =>
        userEventProcessingService.AddUserEventAsync(userEvent: newUserEvent);

    public ValueTask DeleteUserEventAsync(UserEvent deletedUserEvent) =>
        userEventProcessingService.DeleteUserEventAsync(
            userEvent: deletedUserEvent);

    public IQueryable<UserEvent> GetAllUserEvents() =>
        userEventProcessingService.GetAllUserEvents();
}