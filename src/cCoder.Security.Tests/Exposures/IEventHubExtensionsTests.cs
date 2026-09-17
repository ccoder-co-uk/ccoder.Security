// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Security.Data.Models;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Events;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Exposures;

public sealed partial class IEventHubExtensionsTests
{
    [Fact]
    public void SecurityEventStartup_WhenInspected_IsExposedFromIEventHubExtensions()
    {
        // Given
        Type securityAssemblyMarker = typeof(IServiceCollectionExtensions);

        // When
        Type eventHubExtensionsType = securityAssemblyMarker.Assembly
            .GetType(name: "cCoder.Security.IEventHubExtensions");

        // Then
        eventHubExtensionsType
            .Should()
            .NotBeNull();

        eventHubExtensionsType!
            .GetMethods()
            .Where(predicate: method => method.IsPublic && method.IsStatic)
            .Single(predicate: method => method.Name == "ListenToSecurityEvents")
            .GetParameters()[0]
            .ParameterType
            .Should()
            .Be(expected: typeof(IEventHub));
    }

    [Fact]
    public void SecurityEventListening_WhenInspected_HasNoIntermediateHandlerOrBrokerTypes()
    {
        // Given
        Type securityAssemblyMarker = typeof(IServiceCollectionExtensions);

        // When
        Type[] eventListeningInfrastructure = securityAssemblyMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.Namespace?.StartsWith(
                    value: "cCoder.Security.Exposures.EventHandlers",
                    comparisonType: StringComparison.Ordinal) == true ||
                type.Name is "EventHubBroker" or "IEventHubBroker")
            .ToArray();

        // Then
        eventListeningInfrastructure
            .Should()
            .BeEmpty(
                because: "IEventHubExtensions should register listeners directly on the established event hub boundary");
    }

    [Fact]
    public void SecurityEvents_WhenStarted_RegisterExpectedBoundaries()
    {
        // Given
        Mock<IEventHub> eventHubMock = new();

        (string EventName, Type ServiceType)[] expectedRegistrations =
        [
            ("tenant_setup", typeof(IRegistrationAggregationService)),
            ("security_account_registration_created", typeof(IAccountAuditUserEventProcessingService)),
            ("security_account_registration_confirmed", typeof(IAccountAuditUserEventProcessingService)),
            ("security_account_invitation_created", typeof(IAccountAuditUserEventProcessingService)),
            ("security_account_invitation_accepted", typeof(IAccountAuditUserEventProcessingService)),
            ("security_account_password_reset_requested", typeof(IAccountAuditUserEventProcessingService)),
            ("security_token_issued", typeof(IAccountAuditUserEventProcessingService)),
            ("security_authentication_login_succeeded", typeof(IAccountAuditUserEventProcessingService)),
            ("security_authentication_logout_succeeded", typeof(IAccountAuditUserEventProcessingService)),
            ("security_authentication_failed", typeof(IAccountAuditUserEventProcessingService))
        ];

        // When
        eventHubMock.Object.ListenToSecurityEvents();

        // Then
        eventHubMock.Invocations
            .Select(selector: invocation =>
                (
                    EventName: (string)invocation.Arguments[0],
                    ServiceType: invocation.Method.GetGenericArguments()[1]
                ))
            .Should()
            .Equal(
                expectation: expectedRegistrations,
                equalityComparison: (actual, expected) => actual == expected);
    }

    [Fact]
    public async Task TenantSetupEvent_WhenRaised_MapsRegistrationDetailsAsync()
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

        Mock<IEventHub> eventHubMock = new();

        Mock<IRegistrationAggregationService> serviceMock =
            new(behavior: MockBehavior.Strict);

        serviceMock
            .Setup(expression: service =>
                service.SetupRegisterUserAsync(
                    newRegisterUser: It.Is<RegisterUser>(match: registerUser =>
                        registerUser.DisplayName == setupDetails.User.DisplayName &&
                        registerUser.Email == setupDetails.User.Email &&
                        registerUser.Password == setupDetails.User.PasswordHash &&
                        registerUser.PhoneNumber == setupDetails.User.PhoneNumber &&
                        registerUser.Culture == string.Empty &&
                        registerUser.AppId == 0 &&
                        registerUser.TenantId == setupDetails.Tenant.Id &&
                        registerUser.Tenant == setupDetails.Tenant &&
                        registerUser.User == setupDetails.User)))
            .Returns(value: ValueTask.CompletedTask);

        eventHubMock.Object.ListenToSecurityEvents();

        Func<IRegistrationAggregationService, SetupDetails, ValueTask> handler =
            (Func<IRegistrationAggregationService, SetupDetails, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "tenant_setup",
                serviceType: typeof(IRegistrationAggregationService));

        // When
        await handler(
            arg1: serviceMock.Object,
            arg2: setupDetails);

        // Then
        serviceMock.VerifyAll();
        serviceMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("security_account_registration_created")]
    [InlineData("security_account_registration_confirmed")]
    [InlineData("security_account_invitation_created")]
    [InlineData("security_account_invitation_accepted")]
    [InlineData("security_account_password_reset_requested")]
    [InlineData("security_token_issued")]
    [InlineData("security_authentication_login_succeeded")]
    [InlineData("security_authentication_logout_succeeded")]
    [InlineData("security_authentication_failed")]
    public async Task SecurityAccountEvent_WhenRaised_StoresAuditWithExactEventNameAsync(
        string eventName)
    {
        // Given
        SecurityAccountEvent securityAccountEvent = new();
        Mock<IEventHub> eventHubMock = new();

        Mock<IAccountAuditUserEventProcessingService> serviceMock =
            new(behavior: MockBehavior.Strict);

        serviceMock
            .Setup(expression: service =>
                service.StoreSecurityAccountEventAuditAsync(
                    eventName: eventName,
                    securityAccountEvent: securityAccountEvent))
            .Returns(value: ValueTask.CompletedTask);

        eventHubMock.Object.ListenToSecurityEvents();

        Func<IAccountAuditUserEventProcessingService, SecurityAccountEvent, ValueTask> handler =
            (Func<IAccountAuditUserEventProcessingService, SecurityAccountEvent, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: eventName,
                serviceType: typeof(IAccountAuditUserEventProcessingService));

        // When
        await handler(
            arg1: serviceMock.Object,
            arg2: securityAccountEvent);

        // Then
        serviceMock.VerifyAll();
        serviceMock.VerifyNoOtherCalls();
    }

    private static Delegate GetHandler(
        Mock<IEventHub> eventHubMock,
        string eventName,
        Type serviceType) =>
        (Delegate)eventHubMock.Invocations
            .Single(predicate: invocation =>
                invocation.Arguments[0] as string == eventName &&
                invocation.Method.GetGenericArguments()[1] == serviceType)
            .Arguments[1];
}