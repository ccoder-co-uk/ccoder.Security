// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Security.Models.Events;

namespace cCoder.Security.Brokers.Events;

internal interface IAccountEventBroker
{
    ValueTask RaiseAccountEventAsync(string eventName, EventMessage<SecurityAccountEvent> message);
}