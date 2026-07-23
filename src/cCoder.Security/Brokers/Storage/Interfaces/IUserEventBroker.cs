// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface IUserEventBroker
{
    ValueTask<UserEvent> InsertUserEventAsync(UserEvent newUserEvent);
    ValueTask DeleteUserEventAsync(UserEvent deletedUserEvent);
    IQueryable<UserEvent> SelectAllUserEvents();
    ValueTask<UserEvent> UpdateUserEventAsync(UserEvent updatedUserEvent);
}