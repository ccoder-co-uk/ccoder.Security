// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Events;

namespace cCoder.Security.Services.Processings;

internal sealed partial class AccountEventProcessingService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateAccountEventRequestOnRaise(
        SecurityAccountEventRequest accountEventRequest) =>
        Validate(inputs: accountEventRequest);
}