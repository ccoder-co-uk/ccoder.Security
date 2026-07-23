// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Security.Services.Coordinations.Interfaces;

namespace cCoder.Security.Exposures;

internal class TenantManager(ITenantSetupCoordinationService tenantSetupCoordinationService)
    : ITenantManager
{
    public ValueTask SetupAsync(SetupDetails setupDetails) =>
        tenantSetupCoordinationService.SetupDetailsAsync(setupDetails: setupDetails);
}