// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class AuthorizationService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAuthorizationContextOnGet() =>
        Validate(inputs: []);

    private static void ValidatePrivilegeOnEnsure(
        string privilege,
        string tenantId) =>
        Validate(inputs: privilege);

    private static void ValidatePortalPrivilegeOnEnsure(string privilege) =>
        Validate(inputs: privilege);
}