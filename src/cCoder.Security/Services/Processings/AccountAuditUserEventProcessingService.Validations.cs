// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Events;

namespace cCoder.Security.Services.Processings;

internal sealed partial class AccountAuditUserEventProcessingService
{
    private static void ValidateSecurityAccountEventOnStore(
        string eventName,
        SecurityAccountEvent securityAccountEvent)
    {
        if (string.IsNullOrWhiteSpace(value: eventName))
        {
            throw new ArgumentException(
                message: "An event name is required.",
                paramName: nameof(eventName));
        }

        if (securityAccountEvent is null)
        {
            throw new ArgumentNullException(
                paramName: nameof(securityAccountEvent));
        }
    }
}