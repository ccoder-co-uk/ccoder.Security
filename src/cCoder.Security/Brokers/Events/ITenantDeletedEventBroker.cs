// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Eventing.Models;
using cCoder.Security.Models.Events;

namespace cCoder.Security.Brokers.Events;

internal interface ITenantDeletedEventBroker
{
    ValueTask RaiseTenantDeletedEventAsync(
        EventMessage<SecurityAccountEvent> message);
}