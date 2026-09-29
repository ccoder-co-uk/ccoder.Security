// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;

public interface IUserEventProcessingService
{
    ValueTask<UserEvent> AddUserEventAsync(UserEvent userEvent);

    ValueTask DeleteUserEventAsync(UserEvent userEvent);

    IQueryable<UserEvent> GetAllUserEvents();
}