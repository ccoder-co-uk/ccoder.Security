// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Exposures;

public interface IUserEventManager
{
    ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent);

    ValueTask DeleteUserEventAsync(UserEvent userEvent);

    IQueryable<UserEvent> GetAllUserEvents();
}