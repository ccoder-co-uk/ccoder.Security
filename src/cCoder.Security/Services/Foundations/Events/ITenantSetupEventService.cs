// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;

namespace cCoder.Security.Services.Foundations.Events;

internal interface ITenantSetupEventService
{
    ValueTask RaiseSetupAsync(SetupDetails setupDetails);
}