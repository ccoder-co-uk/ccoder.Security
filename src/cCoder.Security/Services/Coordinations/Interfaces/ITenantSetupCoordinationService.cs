// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;

namespace cCoder.Security.Services.Coordinations.Interfaces;

internal interface ITenantSetupCoordinationService
{
    ValueTask SetupDetailsAsync(SetupDetails setupDetails);
}