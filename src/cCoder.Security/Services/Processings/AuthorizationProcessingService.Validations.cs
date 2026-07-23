// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;

namespace cCoder.Security.Services.Processings;

internal sealed partial class AuthorizationProcessingService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateCurrentUserOnGet() =>
        Validate(inputs: []);

    private static void ValidatePrivilegesOnGet() =>
        Validate(inputs: []);

    private static void ValidatePrivilegeOnEnsure(
        string privilege,
        string tenantId) =>
            Validate(inputs: privilege);

    private static void ValidatePortalPrivilegeOnEnsure(string privilege) =>
        Validate(inputs: privilege);
}