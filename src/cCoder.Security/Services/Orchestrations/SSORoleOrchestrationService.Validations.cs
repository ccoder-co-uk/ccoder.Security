// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations;

internal sealed partial class SSORoleOrchestrationService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSSORolesOnGet() =>
        Validate(inputs: []);

    private static void ValidateSSORoleOnAdd(SSORole newSSORole) =>
        Validate(inputs: newSSORole);

    private static void ValidateSSORoleOnUpdate(SSORole updatedSSORole) =>
        Validate(inputs: updatedSSORole);

    private static void ValidateSSORoleOnDelete(SSORole deletedSSORole) =>
        Validate(inputs: deletedSSORole);
}