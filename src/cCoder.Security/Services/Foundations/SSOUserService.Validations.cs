// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class SSOUserService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSSOUserOnAdd(SSOUser newUser) =>
        Validate(inputs: newUser);

    private static void ValidateSSOUserOnUpdate(SSOUser updatedSSOUser) =>
        Validate(inputs: updatedSSOUser);

    private static void ValidateSSOUserOnDelete(SSOUser deletedSSOUser) =>
        Validate(inputs: deletedSSOUser);

    private static void ValidateAllSSOUsersOnGet(bool ignoreFilters) =>
        Validate(inputs: ignoreFilters);
}