// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Events;

namespace cCoder.Security.Services.Processings;

internal sealed partial class AccountEventProcessingService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAccountEventRequestOnRaise(
        SecurityAccountEventRequest accountEventRequest) =>
        Validate(inputs: accountEventRequest);
}