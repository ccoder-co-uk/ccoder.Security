// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;

internal interface ITenantRelationsOrchestrationService
{
    ValueTask DeleteTenantRelationsAsync(Tenant item);
}