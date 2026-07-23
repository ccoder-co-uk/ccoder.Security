// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations;

internal sealed partial class SSOUserRoleOrchestrationService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSSOUserRolesOnGet() =>
        Validate(inputs: []);

    private static void ValidateSSOUserRoleOnAdd(
        SSOUserRole newSSOUserRole) =>
        Validate(inputs: newSSOUserRole);

    private static void ValidateSSOUserRoleOnDelete(
        SSOUserRole deletedSSOUserRole) =>
        Validate(inputs: deletedSSOUserRole);
}