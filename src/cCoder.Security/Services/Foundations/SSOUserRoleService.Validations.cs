// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class SSOUserRoleService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateSSOUserRoleOnAdd(SSOUserRole newSSOUserRole) =>
        Validate(inputs: newSSOUserRole);

    private static void ValidateSSOUserRoleOnDelete(SSOUserRole deletedSSOUserRole) =>
        Validate(inputs: deletedSSOUserRole);
}