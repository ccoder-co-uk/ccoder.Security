// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Events;
using cCoder.Security.Brokers.Logging;
using cCoder.Security.Data.Models;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Services.Aggregations.Interfaces;

namespace cCoder.Security.Exposures.EventHandlers;

internal sealed partial class TenantSetupEventHandlers(
    IEventHubBroker eventHubBroker,
    ILoggingBroker loggingBroker)
        : ISecurityEventHandlers
{
    private readonly IEventHubBroker eventHubBroker =
        eventHubBroker;

    private readonly ILoggingBroker loggingBroker =
        loggingBroker;

    public void ListenToAllEvents() =>
        TryCatch(operation: () =>
            eventHubBroker.ListenToEvent<
                SetupDetails,
                IRegistrationAggregationService>(
                    eventName: "tenant_setup",
                    handler: (service, setupDetails) =>
                    {
                        RegisterUser registerUser = new()
                        {
                            DisplayName =
                                setupDetails.User.DisplayName,
                            Email = setupDetails.User.Email,
                            Password =
                                setupDetails.User.PasswordHash,
                            PhoneNumber =
                                setupDetails.User.PhoneNumber,
                            Culture = string.Empty,
                            AppId = 0,
                            TenantId = setupDetails.Tenant.Id,
                            Tenant = setupDetails.Tenant,
                            User = setupDetails.User
                        };

                        return service.SetupRegisterUserAsync(
                            newRegisterUser: registerUser);
                    }));
}