// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class SSORoleService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllSSORolesOnGet(bool ignoreFilters) =>
        Validate(inputs: ignoreFilters);

    private static void ValidateSSORoleOnAdd(SSORole newSSORole) =>
        Validate(inputs: newSSORole);

    private static void ValidateSSORoleOnUpdate(SSORole updatedSSORole) =>
        Validate(inputs: updatedSSORole);

    private static void ValidateSSORoleOnDelete(SSORole deletedSSORole) =>
        Validate(inputs: deletedSSORole);
}