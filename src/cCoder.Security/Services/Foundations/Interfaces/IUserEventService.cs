// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface IUserEventService
{
    ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent);
    ValueTask DeleteUserEventAsync(UserEvent userEvent);
    IQueryable<UserEvent> GetAllUserEvents();
}