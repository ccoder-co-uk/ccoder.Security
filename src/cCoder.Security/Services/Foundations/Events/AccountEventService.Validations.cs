// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Events;

namespace cCoder.Security.Services.Foundations.Events;

internal sealed partial class AccountEventService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateSecurityAccountEventOnRaise(
        SecurityAccountEventRequest accountEventRequest) =>
        Validate(inputs: accountEventRequest);
}