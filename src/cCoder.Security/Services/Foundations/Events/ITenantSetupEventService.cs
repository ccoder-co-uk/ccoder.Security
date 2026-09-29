// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Security.Data.Models;

namespace cCoder.Security.Services.Foundations.Events;

internal interface ITenantSetupEventService
{
    ValueTask RaiseSetupDetailsAsync(SetupDetails setupDetails);
}