// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class SSOPrivilegeService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePrivilegesOnGet() =>
        Validate(inputs: []);
}