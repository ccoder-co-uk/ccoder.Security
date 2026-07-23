// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface IUserEventService
{
    ValueTask<UserEvent> AddUserEventAsync(UserEvent newUserEvent);
    ValueTask DeleteUserEventAsync(UserEvent deletedUserEvent);
    IQueryable<UserEvent> GetAllUserEvents();
}