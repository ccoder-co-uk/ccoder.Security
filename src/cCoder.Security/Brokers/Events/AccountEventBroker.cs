using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Security.Objects.Events;

namespace cCoder.Security.Brokers.Events;

internal class AccountEventBroker(IEventHub eventHub) : IAccountEventBroker
{
    public ValueTask RaiseAccountEventAsync(string eventName, EventMessage<SecurityAccountEvent> message) =>
        eventHub.RaiseEventAsync(eventName, message);
}
