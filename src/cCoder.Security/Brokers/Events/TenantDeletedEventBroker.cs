// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Security.Models.Events;

namespace cCoder.Security.Brokers.Events;

internal sealed class TenantDeletedEventBroker(
    IEventHub eventHub)
    : ITenantDeletedEventBroker
{
    public ValueTask RaiseTenantDeletedEventAsync(
        EventMessage<SecurityAccountEvent> message) =>
        eventHub.RaiseEventAsync(
            name: "tenant_deleted",
            message: message);
}