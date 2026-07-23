// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Events;
using cCoder.Security.Data.Models;
using cCoder.Security.Services.Processings.Events;

namespace cCoder.Security.Services.Foundations.Events;

internal sealed partial class EventHandlerService(IEventHubBroker eventHubBroker)
    : IEventHandlerService
{
    public void ListenToAllEvents() =>
        TryCatch(operation: () =>
            eventHubBroker.ListenToEvent(
                eventName: "tenant_setup",
                handler: (ITenantSetupEventProcessingService service, SetupDetails details) =>
                    service.SetupDetailsAsync(setupDetails: details)));
}