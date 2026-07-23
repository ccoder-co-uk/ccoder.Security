// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Security.Dependencies;

namespace cCoder.Security.Services.Foundations.Events;

internal sealed partial class TenantSetupEventService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSetupDetailsOnRaise(SetupDetails setupDetails) =>
        Validate(inputs: setupDetails);
}