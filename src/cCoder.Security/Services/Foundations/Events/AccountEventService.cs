// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Security.Brokers.Authentication;
using cCoder.Security.Brokers.Events;
using cCoder.Security.Brokers.Requests;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Events;

namespace cCoder.Security.Services.Foundations.Events;

internal sealed partial class AccountEventService(
    IAccountEventBroker accountEventBroker,
    IHttpRequestBroker requestBroker,
    IAuthenticationContextBroker authenticationContextBroker)
        : IAccountEventService
{
    public ValueTask RaiseSecurityAccountEventRequestAsync(
        SecurityAccountEventRequest securityAccountEventRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateSecurityAccountEventOnRaise(
                accountEventRequest: securityAccountEventRequest);

            SecurityAccountEvent accountEvent = new()
            {
                Kind = securityAccountEventRequest.Kind,
                ActorUserId = ResolveActorUserId(),
                User = securityAccountEventRequest.User,
                Tenant = ResolveTenant(
                    accountEventRequest: securityAccountEventRequest),
                RequestDomain = ResolveRequestDomain(),
                Token = securityAccountEventRequest.Token,
                Culture = securityAccountEventRequest.RegisterForm?.Culture
            };

            EventMessage<SecurityAccountEvent> message = new()
            {
                AuthInfo = new EventAuthInfo
                {
                    SSOUserId = accountEvent.ActorUserId
                },
                Data = accountEvent
            };

            await accountEventBroker.RaiseAccountEventAsync(
                eventName: ResolveEventName(
                    kind: securityAccountEventRequest.Kind),
                message: message);
        });

    private string ResolveRequestDomain()
    {
        string forwardedHost = requestBroker.Header(key: "X-Forwarded-Host");

        return string.IsNullOrWhiteSpace(value: forwardedHost)
            ? requestBroker.RequestHost()
            : forwardedHost;
    }

    private string ResolveActorUserId()
    {
        string actorUserId = authenticationContextBroker.GetSSOUserId();

        return string.Equals(
            a: actorUserId,
            b: "Guest",
            comparisonType: StringComparison.OrdinalIgnoreCase)
                ? null
                : actorUserId;
    }

    private static Tenant ResolveTenant(
        SecurityAccountEventRequest accountEventRequest) =>
        accountEventRequest.Tenant
        ?? accountEventRequest.User?.Roles?
            .Select(selector: userRole => userRole.Role?.Tenant)
            .FirstOrDefault(predicate: tenant => tenant is not null);

    private static string ResolveEventName(SecurityAccountEventKind kind) =>
        kind.ToEventName();
}