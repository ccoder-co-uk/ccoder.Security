using cCoder.Security.Brokers.Events;
using cCoder.Security.Data.Models;
using cCoder.Security.Services.Processings.Events;

namespace cCoder.Security.Services.Foundations.Events;

internal class EventHandlerService(IEventHubBroker eventHubBroker) : IEventHandlerService
{
    public void ListenToAllEvents() =>
        eventHubBroker.ListenToEvent(
            "tenant_setup",
            (ITenantSetupEventProcessingService service, SetupDetails details) => service.SetupAsync(details));
}

