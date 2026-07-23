// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Eventing.Models;

namespace cCoder.Security.Brokers.Events;

internal interface ITenantSetupEventBroker
{
    ValueTask RaiseTenantSetupEventAsync(EventMessage<SetupDetails> message);
}