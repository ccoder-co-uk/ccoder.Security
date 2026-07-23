// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;

namespace cCoder.Security.Services.Processings.Events;

internal interface ITenantSetupEventProcessingService
{
    ValueTask SetupAsync(SetupDetails setupDetails);
}