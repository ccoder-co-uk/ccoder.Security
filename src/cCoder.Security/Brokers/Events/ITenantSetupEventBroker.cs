using cCoder.Security.Data.Models;
using EventLibrary.Models;

namespace cCoder.Security.Brokers.Events;
internal interface ITenantSetupEventBroker
{
    ValueTask RaiseTenantSetupEventAsync(EventMessage<SetupDetails> message);
}

