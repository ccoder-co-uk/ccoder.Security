// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Security.Brokers.Authentication;
using cCoder.Security.Brokers.Events;
using cCoder.Security.Brokers.Requests;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.Events;

namespace cCoder.Security.Services.Foundations.Events;

internal sealed partial class AccountEventService(
    IAccountEventBroker accountEventBroker,
    ITenantBroker tenantBroker,
    IHttpRequestBroker requestBroker,
    IAuthenticationContextBroker authenticationContextBroker)
        : IAccountEventService
{
    public ValueTask RaiseSecurityAccountEventRequestAsync(
        SecurityAccountEventRequest accountEventRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateSecurityAccountEventOnRaise(accountEventRequest: accountEventRequest);

            SecurityAccountEvent accountEvent = new()
            {
                Kind = accountEventRequest.Kind,
                User = accountEventRequest.User,
                Tenant = ResolveTenant(accountEventRequest: accountEventRequest),
                RequestDomain = ResolveRequestDomain(),
                Token = accountEventRequest.Token,
                Culture = accountEventRequest.RegisterForm?.Culture
            };

            EventMessage<SecurityAccountEvent> message = new()
            {
                AuthInfo = new EventAuthInfo
                {
                    SSOUserId = authenticationContextBroker.GetSSOUserId()
                },
                Data = accountEvent
            };

            await accountEventBroker.RaiseAccountEventAsync(
                eventName: ResolveEventName(kind: accountEventRequest.Kind),
                message: message);
        });

    private string ResolveRequestDomain()
    {
        string forwardedHost = requestBroker.Header(key: "X-Forwarded-Host");

        return string.IsNullOrWhiteSpace(value: forwardedHost)
            ? requestBroker.RequestHost()
            : forwardedHost;
    }

    private Tenant ResolveTenant(SecurityAccountEventRequest accountEventRequest)
    {
        string tenantId = accountEventRequest.RegisterForm?.TenantId;

        if (!string.IsNullOrWhiteSpace(value: tenantId))
        {
            return tenantBroker
                .SelectAllTenants()
                .FirstOrDefault(predicate: tenant => tenant.Id == tenantId);
        }

        return accountEventRequest.User?.Roles?
            .Select(selector: userRole => userRole.Role?.Tenant)
            .FirstOrDefault(predicate: tenant => tenant is not null);
    }

    private static string ResolveEventName(SecurityAccountEventKind kind) =>
        kind switch
        {
            SecurityAccountEventKind.RegistrationCreated =>
                SecurityAccountEventNames.RegistrationCreated,
            SecurityAccountEventKind.RegistrationConfirmed =>
                SecurityAccountEventNames.RegistrationConfirmed,
            SecurityAccountEventKind.InvitationCreated =>
                SecurityAccountEventNames.InvitationCreated,
            SecurityAccountEventKind.InvitationAccepted =>
                SecurityAccountEventNames.InvitationAccepted,
            SecurityAccountEventKind.PasswordResetRequested =>
                SecurityAccountEventNames.PasswordResetRequested,
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
}