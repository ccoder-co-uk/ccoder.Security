// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Events;
using cCoder.Security.Services.Foundations.Events;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class AccountEventProcessingService(
    IAccountEventService accountEventService)
        : IAccountEventProcessingService
{
    public ValueTask RaiseSecurityAccountEventRequestAsync(
        SecurityAccountEventRequest accountEventRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateAccountEventRequestOnRaise(
                accountEventRequest: accountEventRequest);

            await accountEventService.RaiseSecurityAccountEventRequestAsync(
                accountEventRequest: accountEventRequest);
        });
}