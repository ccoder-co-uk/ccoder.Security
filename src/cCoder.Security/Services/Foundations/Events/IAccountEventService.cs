// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Security.Models.Events;

namespace cCoder.Security.Services.Foundations.Events;

internal interface IAccountEventService
{
    ValueTask RaiseSecurityAccountEventRequestAsync(SecurityAccountEventRequest accountEventRequest);
}