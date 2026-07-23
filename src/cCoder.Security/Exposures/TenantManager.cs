// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Security.Services.Managements.Interfaces;

namespace cCoder.Security.Exposures;

internal class TenantManager(ITenantSetupManagementService tenantSetupManagementService)
    : ITenantManager
{
    public ValueTask SetupAsync(SetupDetails setupDetails) =>
        tenantSetupManagementService.SetupDetailsAsync(setupDetails: setupDetails);
}