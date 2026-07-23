// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public ValueTask RaiseRegistrationCreatedSSOUserRegisterUserEventAsync(
        SSOUser user,
        RegisterUser registerForm,
        string token) =>
        RaiseAsync(
eventName: SecurityAccountEventNames.RegistrationCreated,
kind: SecurityAccountEventKind.RegistrationCreated,
user: user,
tenant: ResolveTenant(tenantId: registerForm?.TenantId),
token: token,
culture: registerForm?.Culture);

    public ValueTask RaiseRegistrationConfirmedSSOUserEventAsync(SSOUser user, string token) =>
        RaiseAsync(
eventName: SecurityAccountEventNames.RegistrationConfirmed,
kind: SecurityAccountEventKind.RegistrationConfirmed,
user: user,
tenant: ResolveTenant(user: user),
token: token,
culture: null);

    public ValueTask RaiseInvitationCreatedSSOUserRegisterUserEventAsync(
        SSOUser user,
        RegisterUser registerForm,
        string token) =>
        RaiseAsync(
eventName: SecurityAccountEventNames.InvitationCreated,
kind: SecurityAccountEventKind.InvitationCreated,
user: user,
tenant: ResolveTenant(tenantId: registerForm?.TenantId),
token: token,
culture: registerForm?.Culture);

    public ValueTask RaiseInvitationAcceptedSSOUserRegisterUserEventAsync(
        SSOUser user,
        RegisterUser registerForm,
        string token) =>
        RaiseAsync(
eventName: SecurityAccountEventNames.InvitationAccepted,
kind: SecurityAccountEventKind.InvitationAccepted,
user: user,
tenant: ResolveTenant(tenantId: registerForm?.TenantId) ?? ResolveTenant(user: user),
token: token,
culture: registerForm?.Culture);

    public ValueTask RaisePasswordResetRequestedSSOUserEventAsync(SSOUser user, string token) =>
        RaiseAsync(
eventName: SecurityAccountEventNames.PasswordResetRequested,
kind: SecurityAccountEventKind.PasswordResetRequested,
user: user,
tenant: ResolveTenant(user: user),
token: token,
culture: null);

    private ValueTask RaiseAsync(
        string eventName,
        SecurityAccountEventKind kind,
        SSOUser user,
        Tenant tenant,
        string token,
        string culture) =>
        accountEventBroker.RaiseAccountEventAsync(
eventName: eventName,
message: new EventMessage<SecurityAccountEvent>
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
        RequestDomain = ResolveRequestDomain(),
        Token = token,
        Culture = culture
    }
});

    private string ResolveRequestDomain()
    {
        string forwardedHost = requestBroker.Header(key: "X-Forwarded-Host");

        return string.IsNullOrWhiteSpace(value: forwardedHost)
            ? requestBroker.RequestHost()
            : forwardedHost;
    }

    private Tenant ResolveTenant(string tenantId) =>
        string.IsNullOrWhiteSpace(value: tenantId)
            ? null
            : tenantProcessingService.GetAllTenants()
                .FirstOrDefault(predicate: tenant => tenant.Id == tenantId);

    private Tenant ResolveTenant(SSOUser user) =>
        user?.Roles?
            .Select(selector: userRole => userRole.Role?.Tenant)
            .FirstOrDefault(predicate: tenant => tenant is not null);
}