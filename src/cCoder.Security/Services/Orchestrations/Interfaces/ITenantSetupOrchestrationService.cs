// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;

namespace cCoder.Security.Services.Orchestrations.Interfaces;

internal interface ITenantSetupOrchestrationService
{
    ValueTask SetupDetailsAsync(SetupDetails setupDetails);
}