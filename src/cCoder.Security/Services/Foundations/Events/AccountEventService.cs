using cCoder.Eventing.Models;
using cCoder.Security.Brokers.Events;
using cCoder.Security.Brokers.Requests;
using cCoder.Security.Objects;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.Events;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Foundations.Events;

internal class AccountEventService(
    IAccountEventBroker accountEventBroker,
    ITenantProcessingService tenantProcessingService,
    IHttpRequestBroker requestBroker,
    ISSOAuthInfo authInfo) : IAccountEventService
{
    public ValueTask RaiseRegistrationCreatedEventAsync(SSOUser user, RegisterUser registerForm, string token) =>
        RaiseAsync(
            SecurityAccountEventNames.RegistrationCreated,
            SecurityAccountEventKind.RegistrationCreated,
            user,
            ResolveTenant(registerForm?.TenantId),
            token,
            registerForm?.Culture);

    public ValueTask RaiseRegistrationConfirmedEventAsync(SSOUser user, string token) =>
        RaiseAsync(
            SecurityAccountEventNames.RegistrationConfirmed,
            SecurityAccountEventKind.RegistrationConfirmed,
            user,
            ResolveTenant(user),
            token,
            null);

    public ValueTask RaiseInvitationCreatedEventAsync(SSOUser user, RegisterUser registerForm, string token) =>
        RaiseAsync(
            SecurityAccountEventNames.InvitationCreated,
            SecurityAccountEventKind.InvitationCreated,
            user,
            ResolveTenant(registerForm?.TenantId),
            token,
            registerForm?.Culture);

    public ValueTask RaiseInvitationAcceptedEventAsync(SSOUser user, RegisterUser registerForm, string token) =>
        RaiseAsync(
            SecurityAccountEventNames.InvitationAccepted,
            SecurityAccountEventKind.InvitationAccepted,
            user,
            ResolveTenant(registerForm?.TenantId) ?? ResolveTenant(user),
            token,
            registerForm?.Culture);

    public ValueTask RaisePasswordResetRequestedEventAsync(SSOUser user, string token) =>
        RaiseAsync(
            SecurityAccountEventNames.PasswordResetRequested,
            SecurityAccountEventKind.PasswordResetRequested,
            user,
            ResolveTenant(user),
            token,
            null);

    private ValueTask RaiseAsync(
        string eventName,
        SecurityAccountEventKind kind,
        SSOUser user,
        Tenant tenant,
        string token,
        string culture) =>
        accountEventBroker.RaiseAccountEventAsync(
            eventName,
            new EventMessage<SecurityAccountEvent>
            {
                AuthInfo = new EventAuthInfo
                {
                    SSOUserId = authInfo?.SSOUserId ?? "Guest"
                },
                Data = new SecurityAccountEvent
                {
                    Kind = kind,
                    User = user,
                    Tenant = tenant,
                    RequestDomain = requestBroker.GetRequestDomain(),
                    Token = token,
                    Culture = culture
                }
            });

    private Tenant ResolveTenant(string tenantId) =>
        string.IsNullOrWhiteSpace(tenantId)
            ? null
            : tenantProcessingService.GetAllTenants()
                .FirstOrDefault(tenant => tenant.Id == tenantId);

    private Tenant ResolveTenant(SSOUser user) =>
        user?.Roles?
            .Select(userRole => userRole.Role?.Tenant)
            .FirstOrDefault(tenant => tenant is not null);
}
