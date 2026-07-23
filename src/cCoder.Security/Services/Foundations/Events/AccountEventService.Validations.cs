// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Events;

namespace cCoder.Security.Services.Foundations.Events;

internal sealed partial class AccountEventService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSecurityAccountEventOnRaise(
        SecurityAccountEventRequest accountEventRequest) =>
        Validate(inputs: accountEventRequest);
}