// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings;

internal sealed partial class TenantProcessingService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateTenantOnAdd(Tenant newTenant) =>
        Validate(inputs: newTenant);

    private static void ValidateTenantOnDelete(Tenant deletedTenant) =>
        Validate(inputs: deletedTenant);

    private static void ValidateTenantsOnGet() =>
        Validate(inputs: []);

    private static void ValidateTenantOnUpdate(Tenant updatedTenant) =>
        Validate(inputs: updatedTenant);
}