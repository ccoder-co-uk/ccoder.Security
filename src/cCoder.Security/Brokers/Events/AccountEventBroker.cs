// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Security.Models.Events;

namespace cCoder.Security.Brokers.Events;

internal class AccountEventBroker(IEventHub eventHub) : IAccountEventBroker
{
    public ValueTask RaiseAccountEventAsync(string eventName, EventMessage<SecurityAccountEvent> message) =>
        eventHub.RaiseEventAsync(name: eventName, message: message);
}