// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Events;
using cCoder.Security.Data.Models;
using cCoder.Security.Services.Processings.Events;

namespace cCoder.Security.Services.Foundations.Events;

internal class EventHandlerService(IEventHubBroker eventHubBroker) : IEventHandlerService
{
    public void ListenToAllEvents() =>
        eventHubBroker.ListenToEvent(
eventName: "tenant_setup",
handler: (ITenantSetupEventProcessingService service, SetupDetails details) =>
    service.SetupDetailsAsync(setupDetails: details));
}