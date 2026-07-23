// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;

namespace cCoder.Security.Services.Managements.Interfaces;

internal interface ITenantSetupManagementService
{
    ValueTask SetupDetailsAsync(SetupDetails setupDetails);
}