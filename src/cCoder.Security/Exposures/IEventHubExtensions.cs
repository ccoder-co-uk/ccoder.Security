// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Security.Data.Models;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Events;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security;

public static class IEventHubExtensions
{
    public static IEventHub ListenToSecurityEvents(this IEventHub eventHub)
    {
        ListenToTenantSetupEvent(eventHub: eventHub);
        ListenToAccountAuditEvents(eventHub: eventHub);

        return eventHub;
    }

    private static void ListenToTenantSetupEvent(IEventHub eventHub) =>
        eventHub.ListenToEvent(
            name: "tenant_setup",
            handler: (IRegistrationAggregationService service, SetupDetails setupDetails) =>
            {
                RegisterUser registerUser = new()
                {
                    DisplayName = setupDetails.User.DisplayName,
                    Email = setupDetails.User.Email,
                    Password = setupDetails.User.PasswordHash,
                    PhoneNumber = setupDetails.User.PhoneNumber,
                    Culture = string.Empty,
                    AppId = 0,
                    TenantId = setupDetails.Tenant.Id,
                    Tenant = setupDetails.Tenant,
                    User = setupDetails.User
                };

                return service.SetupRegisterUserAsync(
                    newRegisterUser: registerUser);
            });

    private static void ListenToAccountAuditEvents(IEventHub eventHub)
    {
        ListenToAccountAuditEvent(
            eventHub: eventHub,
            eventName: "security_account_registration_created");

        ListenToAccountAuditEvent(
            eventHub: eventHub,
            eventName: "security_account_registration_confirmed");

        ListenToAccountAuditEvent(
            eventHub: eventHub,
            eventName: "security_account_invitation_created");

        ListenToAccountAuditEvent(
            eventHub: eventHub,
            eventName: "security_account_invitation_accepted");

        ListenToAccountAuditEvent(
            eventHub: eventHub,
            eventName: "security_account_password_reset_requested");

        ListenToAccountAuditEvent(
            eventHub: eventHub,
            eventName: "security_token_issued");

        ListenToAccountAuditEvent(
            eventHub: eventHub,
            eventName: "security_authentication_login_succeeded");

        ListenToAccountAuditEvent(
            eventHub: eventHub,
            eventName: "security_authentication_logout_succeeded");

        ListenToAccountAuditEvent(
            eventHub: eventHub,
            eventName: "security_authentication_failed");
    }

    private static void ListenToAccountAuditEvent(
        IEventHub eventHub,
        string eventName) =>
        eventHub.ListenToEvent(
            name: eventName,
            handler: (IAccountAuditUserEventProcessingService service,
                SecurityAccountEvent securityAccountEvent) =>
                    service.StoreSecurityAccountEventAuditAsync(
                        eventName: eventName,
                        securityAccountEvent: securityAccountEvent));
}