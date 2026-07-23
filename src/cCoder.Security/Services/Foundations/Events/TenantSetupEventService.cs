// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Authentication;
using cCoder.Security.Brokers.Events;
using cCoder.Security.Data.Models;
using cCoder.Eventing.Models;

namespace cCoder.Security.Services.Foundations.Events;

internal sealed partial class TenantSetupEventService(
    ITenantSetupEventBroker tenantSetupEventBroker,
    IAuthenticationContextBroker authenticationContextBroker)
        : ITenantSetupEventService
{
    public ValueTask RaiseSetupDetailsAsync(SetupDetails setupDetails) =>
        TryCatch(operation: async () =>
        {
            ValidateSetupDetailsOnRaise(setupDetails: setupDetails);

            EventMessage<SetupDetails> message = new()
            {
                AuthInfo = new EventAuthInfo
                {
                    SSOUserId = authenticationContextBroker.GetSSOUserId()
                },
                Data = setupDetails
            };

            await tenantSetupEventBroker.RaiseTenantSetupEventAsync(message: message);
        });
}