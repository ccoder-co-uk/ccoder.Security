// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Events;

namespace cCoder.Security.Services.Foundations.Events;

internal interface IAccountEventService
{
    ValueTask RaiseSecurityAccountEventRequestAsync(SecurityAccountEventRequest accountEventRequest);
}