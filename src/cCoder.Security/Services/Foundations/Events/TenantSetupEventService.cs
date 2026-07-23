// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Events;
using cCoder.Security.Data.Models;
using cCoder.Security.Objects;
using cCoder.Eventing.Models;

namespace cCoder.Security.Services.Foundations.Events;

internal class TenantSetupEventService(
    ITenantSetupEventBroker tenantSetupEventBroker,
    ISSOAuthInfo authInfo) : ITenantSetupEventService
{
    public ValueTask RaiseSetupAsync(SetupDetails setupDetails) =>
        tenantSetupEventBroker.RaiseTenantSetupEventAsync(
message: new EventMessage<SetupDetails>
{
    AuthInfo = new EventAuthInfo
    {
        SSOUserId = authInfo?.SSOUserId ?? "Guest"
    },
    Data = setupDetails
});
}