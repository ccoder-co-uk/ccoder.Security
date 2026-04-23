using cCoder.Security.Data.Models;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.Security.Brokers.Events;

internal class TenantSetupEventBroker(IEventHub eventHub) : ITenantSetupEventBroker
{
    public ValueTask RaiseTenantSetupEventAsync(EventMessage<SetupDetails> message) =>
        eventHub.RaiseEventAsync("tenant_setup", message);
}
