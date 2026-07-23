// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Security.Services.Processings.Events;

namespace cCoder.Security.Exposures;

internal class TenantManager(ITenantSetupEventProcessingService tenantSetupEventProcessingService) : ITenantManager
{
    public ValueTask SetupAsync(SetupDetails setupDetails) =>
        tenantSetupEventProcessingService.SetupAsync(setupDetails: setupDetails);
}