// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Events;
using cCoder.Security.Brokers.Logging;
using cCoder.Security.Models.Events;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Exposures.EventHandlers;

internal sealed partial class LogoutSucceededEventHandlers(
    IEventHubBroker eventHubBroker,
    ILoggingBroker loggingBroker)
        : ISecurityEventHandlers
{
    private readonly IEventHubBroker eventHubBroker =
        eventHubBroker;

    private readonly ILoggingBroker loggingBroker =
        loggingBroker;

    private const string EventName =
        "security_authentication_logout_succeeded";

    public void ListenToAllEvents() =>
        TryCatch(operation: () =>
            eventHubBroker.ListenToEvent<
                SecurityAccountEvent,
                IAccountAuditUserEventProcessingService>(
                    eventName: EventName,
                    handler: (service, securityAccountEvent) =>
                        service.StoreSecurityAccountEventAuditAsync(
                            eventName: EventName,
                            securityAccountEvent:
                                securityAccountEvent)));
}