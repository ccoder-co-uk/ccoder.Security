// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Events;

namespace cCoder.Security.Services.Processings.Interfaces;

internal interface IAccountEventProcessingService
{
    ValueTask RaiseSecurityAccountEventRequestAsync(
        SecurityAccountEventRequest accountEventRequest);
}