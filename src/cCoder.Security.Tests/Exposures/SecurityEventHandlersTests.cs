// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Events;
using cCoder.Security.Brokers.Logging;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures.EventHandlers;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Events;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using FluentAssertions;
using Moq;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace cCoder.Security.Tests.Exposures;

public sealed partial class SecurityEventHandlersTests
{
    [Theory]
    [InlineData(typeof(RegistrationCreatedEventHandlers), "security_account_registration_created")]
    [InlineData(typeof(RegistrationConfirmedEventHandlers), "security_account_registration_confirmed")]
    [InlineData(typeof(InvitationCreatedEventHandlers), "security_account_invitation_created")]
    [InlineData(typeof(InvitationAcceptedEventHandlers), "security_account_invitation_accepted")]
    [InlineData(typeof(PasswordResetRequestedEventHandlers), "security_account_password_reset_requested")]
    [InlineData(typeof(TokenIssuedEventHandlers), "security_token_issued")]
    [InlineData(typeof(LoginSucceededEventHandlers), "security_authentication_login_succeeded")]
    [InlineData(typeof(LogoutSucceededEventHandlers), "security_authentication_logout_succeeded")]
    [InlineData(typeof(AuthenticationFailedEventHandlers), "security_authentication_failed")]
    public async Task AccountEventHandler_WhenListening_ShouldRegisterExactEvent(
        Type handlerType,
        string eventName)
    {
        // Given
        SecurityAccountEvent securityAccountEvent = new();

        Func<
            IAccountAuditUserEventProcessingService,
            SecurityAccountEvent,
            ValueTask> registeredHandler = null;

        Mock<IEventHubBroker> eventHubBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<IAccountAuditUserEventProcessingService> serviceMock =
            new(behavior: MockBehavior.Strict);

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<
                SecurityAccountEvent,
                IAccountAuditUserEventProcessingService>(
                    eventName: eventName,
                    handler: It.IsAny<Func<
                        IAccountAuditUserEventProcessingService,
                        SecurityAccountEvent,
                        ValueTask>>()))
            .Callback<
                string,
                Func<
                    IAccountAuditUserEventProcessingService,
                    SecurityAccountEvent,
                    ValueTask>>(
                        action: (_, handler) =>
                            registeredHandler = handler);

        serviceMock
            .Setup(expression: service =>
                service.StoreSecurityAccountEventAuditAsync(
                    eventName: eventName,
                    securityAccountEvent: securityAccountEvent))
            .Returns(value: ValueTask.CompletedTask);

        ISecurityEventHandlers eventHandlers =
            CreateEventHandlers(
                handlerType: handlerType,
                eventHubBroker: eventHubBrokerMock.Object);

        // When
        eventHandlers.ListenToAllEvents();

        await registeredHandler(
            arg1: serviceMock.Object,
            arg2: securityAccountEvent);

        // Then
        eventHubBrokerMock.VerifyAll();
        eventHubBrokerMock.VerifyNoOtherCalls();
        serviceMock.VerifyAll();
        serviceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TenantSetupEventHandler_WhenListening_ShouldUseInternalService()
    {
        // Given
        SetupDetails setupDetails = new()
        {
            Tenant = new() { Id = "tenant-id" },
            User = new()
            {
                Id = "user-id",
                DisplayName = "Display Name",
                Email = "user@example.test",
                PasswordHash = "password-hash",
                PhoneNumber = "0123456789"
            }
        };

        Func<
            IRegistrationAggregationService,
            SetupDetails,
            ValueTask> registeredHandler = null;

        Mock<IEventHubBroker> eventHubBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<IRegistrationAggregationService> serviceMock =
            new(behavior: MockBehavior.Strict);

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<
                SetupDetails,
                IRegistrationAggregationService>(
                    eventName: "tenant_setup",
                    handler: It.IsAny<Func<
                        IRegistrationAggregationService,
                        SetupDetails,
                        ValueTask>>()))
            .Callback<
                string,
                Func<
                    IRegistrationAggregationService,
                    SetupDetails,
                    ValueTask>>(
                        action: (_, handler) =>
                            registeredHandler = handler);

        serviceMock
            .Setup(expression: service =>
                service.SetupRegisterUserAsync(
                    newRegisterUser: It.Is<RegisterUser>(match: registerUser =>
                        registerUser.DisplayName ==
                            setupDetails.User.DisplayName
                        && registerUser.Email ==
                            setupDetails.User.Email
                        && registerUser.Password ==
                            setupDetails.User.PasswordHash
                        && registerUser.PhoneNumber ==
                            setupDetails.User.PhoneNumber
                        && registerUser.TenantId ==
                            setupDetails.Tenant.Id
                        && registerUser.Tenant ==
                            setupDetails.Tenant
                        && registerUser.User ==
                            setupDetails.User)))
            .Returns(value: ValueTask.CompletedTask);

        ISecurityEventHandlers eventHandlers =
            CreateEventHandlers(
                handlerType: typeof(TenantSetupEventHandlers),
                eventHubBroker: eventHubBrokerMock.Object);

        // When
        eventHandlers.ListenToAllEvents();

        await registeredHandler(
            arg1: serviceMock.Object,
            arg2: setupDetails);

        // Then
        eventHubBrokerMock.VerifyAll();
        eventHubBrokerMock.VerifyNoOtherCalls();
        serviceMock.VerifyAll();
        serviceMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(typeof(ValidationException), typeof(SecurityValidationException))]
    [InlineData(typeof(InvalidOperationException), typeof(SecurityDependencyException))]
    [InlineData(typeof(Exception), typeof(SecurityServiceException))]
    public void EventHandler_WhenRegistrationFails_ShouldTranslateAndLog(
        Type registrationExceptionType,
        Type translatedExceptionType)
    {
        // Given
        Exception registrationException =
            (Exception)Activator.CreateInstance(
                type: registrationExceptionType);

        Mock<IEventHubBroker> eventHubBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<ILoggingBroker> loggingBrokerMock =
            new(behavior: MockBehavior.Strict);

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<
                SetupDetails,
                IRegistrationAggregationService>(
                    eventName: "tenant_setup",
                    handler: It.IsAny<Func<
                        IRegistrationAggregationService,
                        SetupDetails,
                        ValueTask>>()))
            .Throws(exception: registrationException);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogException(
                exception: registrationException));

        ISecurityEventHandlers eventHandlers =
            CreateEventHandlers(
                handlerType: typeof(TenantSetupEventHandlers),
                eventHubBroker: eventHubBrokerMock.Object,
                loggingBroker: loggingBrokerMock.Object);

        // When
        Exception translatedException = Record.Exception(
            testCode: eventHandlers.ListenToAllEvents);

        // Then
        translatedException.Should()
            .BeOfType(expectedType: translatedExceptionType);

        translatedException.InnerException.Should()
            .BeSameAs(expected: registrationException);

        eventHubBrokerMock.VerifyAll();
        loggingBrokerMock.VerifyAll();
    }

    [Theory]
    [InlineData(typeof(RegistrationCreatedEventHandlers), "security_account_registration_created")]
    [InlineData(typeof(RegistrationConfirmedEventHandlers), "security_account_registration_confirmed")]
    [InlineData(typeof(InvitationCreatedEventHandlers), "security_account_invitation_created")]
    [InlineData(typeof(InvitationAcceptedEventHandlers), "security_account_invitation_accepted")]
    [InlineData(typeof(PasswordResetRequestedEventHandlers), "security_account_password_reset_requested")]
    [InlineData(typeof(TokenIssuedEventHandlers), "security_token_issued")]
    [InlineData(typeof(LoginSucceededEventHandlers), "security_authentication_login_succeeded")]
    [InlineData(typeof(LogoutSucceededEventHandlers), "security_authentication_logout_succeeded")]
    [InlineData(typeof(AuthenticationFailedEventHandlers), "security_authentication_failed")]
    public void AccountEventHandler_WhenRegistrationFails_ShouldTranslateAllCategories(
        Type handlerType,
        string eventName)
    {
        // Given
        VerifyAccountRegistrationException(
            handlerType: handlerType,
            eventName: eventName,
            registrationException: new ValidationException(),
            translatedExceptionType: typeof(SecurityValidationException));

        // When
        VerifyAccountRegistrationException(
            handlerType: handlerType,
            eventName: eventName,
            registrationException: new InvalidOperationException(),
            translatedExceptionType: typeof(SecurityDependencyException));

        // Then
        VerifyAccountRegistrationException(
            handlerType: handlerType,
            eventName: eventName,
            registrationException: new Exception(),
            translatedExceptionType: typeof(SecurityServiceException));
    }

    private static ISecurityEventHandlers CreateEventHandlers(
        Type handlerType,
        IEventHubBroker eventHubBroker,
        ILoggingBroker loggingBroker = null) =>
        (ISecurityEventHandlers)Activator.CreateInstance(
            type: handlerType,
            args:
            [
                eventHubBroker,
                loggingBroker ?? Mock.Of<ILoggingBroker>()
            ]);

    private static void VerifyAccountRegistrationException(
        Type handlerType,
        string eventName,
        Exception registrationException,
        Type translatedExceptionType)
    {
        Mock<IEventHubBroker> eventHubBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<ILoggingBroker> loggingBrokerMock =
            new(behavior: MockBehavior.Strict);

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<
                SecurityAccountEvent,
                IAccountAuditUserEventProcessingService>(
                    eventName: eventName,
                    handler: It.IsAny<Func<
                        IAccountAuditUserEventProcessingService,
                        SecurityAccountEvent,
                        ValueTask>>()))
            .Throws(exception: registrationException);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogException(
                exception: registrationException));

        ISecurityEventHandlers eventHandlers = CreateEventHandlers(
            handlerType: handlerType,
            eventHubBroker: eventHubBrokerMock.Object,
            loggingBroker: loggingBrokerMock.Object);

        Exception translatedException = Record.Exception(
            testCode: eventHandlers.ListenToAllEvents);

        translatedException.Should()
            .BeOfType(expectedType: translatedExceptionType);

        translatedException.InnerException.Should()
            .BeSameAs(expected: registrationException);

        eventHubBrokerMock.VerifyAll();
        loggingBrokerMock.VerifyAll();
    }
}