// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;
using cCoder.Security.Brokers.Events;
using cCoder.Security.Data.Models;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Events;
using cCoder.Security.Services.Foundations.Events;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class EventHandlerServiceTests
{
    [Fact]
    public void ShouldOnlySubscribeAuditStorageToExplicitSecurityEvents()
    {
        // Given
        Mock<IEventHubBroker> eventHubBrokerMock = new(behavior: MockBehavior.Default);
        EventHandlerService service = new(eventHubBroker: eventHubBrokerMock.Object);

        // When
        service.ListenToAllEvents();

        // Then
        eventHubBrokerMock.Verify(expression: broker =>
            broker.ListenToEvent<SetupDetails, ITenantManager>(
                eventName: "tenant_setup",
                handler: It.IsAny<Func<ITenantManager, SetupDetails, ValueTask>>()),
            times: Times.Once());

        foreach (SecurityAccountEventKind kind in Enum.GetValues<SecurityAccountEventKind>())
        {
            string eventName = kind.ToEventName();

            eventHubBrokerMock.Verify(expression: broker =>
                broker.ListenToEvent<SecurityAccountEvent, IUserEventManager>(
                    eventName: eventName,
                    handler: It.IsAny<Func<IUserEventManager, SecurityAccountEvent, ValueTask>>()),
                times: Times.Once());
        }

        eventHubBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(SecurityAccountEventKind.RegistrationCreated)]
    [InlineData(SecurityAccountEventKind.RegistrationConfirmed)]
    [InlineData(SecurityAccountEventKind.InvitationCreated)]
    [InlineData(SecurityAccountEventKind.InvitationAccepted)]
    [InlineData(SecurityAccountEventKind.PasswordResetRequested)]
    [InlineData(SecurityAccountEventKind.TokenIssued)]
    [InlineData(SecurityAccountEventKind.LoginSucceeded)]
    [InlineData(SecurityAccountEventKind.LogoutSucceeded)]
    [InlineData(SecurityAccountEventKind.AuthenticationFailed)]
    public async Task ShouldStoreRedactedAuditForEveryAccountEvent(
        SecurityAccountEventKind kind)
    {
        // Given
        const string actorUserId = "actor-user";
        const string subjectUserId = "subject-user";
        const string password = "never-store-password";
        const string token = "never-store-token";
        const string tenantId = "tenant-id";
        const string requestDomain = "example.test";
        const string culture = "en-GB";
        string eventName = kind.ToEventName();
        UserEvent storedUserEvent = null;
        Mock<IUserEventManager> managerMock = new(behavior: MockBehavior.Default);

        managerMock
            .Setup(expression: manager => manager.AddUserEventAsync(
                userEvent: It.IsAny<UserEvent>()))
            .Callback<UserEvent>(action: userEvent => storedUserEvent = userEvent)
            .ReturnsAsync(value: new UserEvent());

        SecurityAccountEvent accountEvent = new()
        {
            Kind = kind,
            ActorUserId = actorUserId,
            User = new SSOUser
            {
                Id = subjectUserId,
                Email = "sensitive@example.test",
                PasswordHash = password
            },
            Tenant = new Tenant { Id = tenantId },
            RequestDomain = requestDomain,
            Culture = culture,
            Token = token
        };

        // When
        await EventHandlerService.StoreAccountAuditEventAsync(
            manager: managerMock.Object,
            eventName: eventName,
            accountEvent: accountEvent);

        // Then
        storedUserEvent.Should()
            .NotBeNull();

        storedUserEvent.Id.Should()
            .NotBeEmpty();

        storedUserEvent.EventName.Should()
            .Be(expected: eventName);

        storedUserEvent.CreatedBy.Should()
            .Be(expected: actorUserId);

        storedUserEvent.TenantId.Should()
            .Be(expected: tenantId);

        storedUserEvent.Value.Should()
            .Contain(expected: subjectUserId);

        storedUserEvent.Value.Should()
            .Contain(expected: requestDomain);

        storedUserEvent.Value.Should()
            .Contain(expected: culture);

        storedUserEvent.Value.Should()
            .NotContain(unexpected: token);

        storedUserEvent.Value.Should()
            .NotContain(unexpected: password);

        storedUserEvent.Value.Should()
            .NotContain(unexpected: "sensitive@example.test");
    }

    [Fact]
    public async Task ShouldUseSubjectAsActorForAnonymousAccountEvent()
    {
        // Given
        const string subjectUserId = "self-registering-user";
        Mock<IUserEventManager> managerMock = new(behavior: MockBehavior.Default);
        UserEvent storedUserEvent = null;

        managerMock
            .Setup(expression: manager => manager.AddUserEventAsync(
                userEvent: It.IsAny<UserEvent>()))
            .Callback<UserEvent>(action: userEvent => storedUserEvent = userEvent)
            .ReturnsAsync(value: new UserEvent());

        // When
        await EventHandlerService.StoreAccountAuditEventAsync(
            manager: managerMock.Object,
            eventName: SecurityAccountEventKind.RegistrationCreated.ToEventName(),
            accountEvent: new SecurityAccountEvent
            {
                Kind = SecurityAccountEventKind.RegistrationCreated,
                User = new SSOUser { Id = subjectUserId }
            });

        // Then
        storedUserEvent.CreatedBy.Should()
            .Be(expected: subjectUserId);
    }

}