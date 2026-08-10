// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Services.Processings;

internal sealed partial class AuthorizationProcessingService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidatePrivilegeOnEnsure(
        string privilege,
        string tenantId) =>
        Validate(inputs: privilege);

    private static void ValidatePortalPrivilegeOnEnsure(string privilege) =>
        Validate(inputs: privilege);
}