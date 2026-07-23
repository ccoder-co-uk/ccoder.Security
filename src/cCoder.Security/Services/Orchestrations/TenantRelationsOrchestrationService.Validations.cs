// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations;

internal sealed partial class TenantRelationsOrchestrationService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateTenantRelationsOnDelete(Tenant deletedTenant) =>
        Validate(inputs: deletedTenant);
}