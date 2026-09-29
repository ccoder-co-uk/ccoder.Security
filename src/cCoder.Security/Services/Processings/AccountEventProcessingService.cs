// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Security.Models.Events;
using cCoder.Security.Services.Foundations.Events;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class AccountEventProcessingService(
    IAccountEventService accountEventService)
        : IAccountEventProcessingService
{
    public ValueTask RaiseSecurityAccountEventRequestAsync(
        SecurityAccountEventRequest securityAccountEventRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateAccountEventRequestOnRaise(
                accountEventRequest: securityAccountEventRequest);

            await accountEventService.RaiseSecurityAccountEventRequestAsync(
                accountEventRequest: securityAccountEventRequest);
        });
}