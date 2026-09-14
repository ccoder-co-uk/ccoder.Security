// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Serialization;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Events;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings;

public sealed partial class AccountAuditUserEventProcessingServiceTests
{
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
    public async Task StoreAccountAuditEventAsync_ShouldStoreRedactedAudit(
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

        Mock<IUserEventService> serviceMock =
            new(behavior: MockBehavior.Strict);

        serviceMock
            .Setup(expression: service => service.AddUserEventAsync(
                userEvent: It.IsAny<UserEvent>()))
            .Callback<UserEvent>(action: userEvent =>
                storedUserEvent = userEvent)
            .ReturnsAsync(value: new UserEvent());

        AccountAuditUserEventProcessingService service = new(
            userEventService: serviceMock.Object,
            serializationBroker: new SerializationBroker());

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
        await service.StoreSecurityAccountEventAuditAsync(
            eventName: eventName,
            securityAccountEvent: accountEvent);

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

        serviceMock.VerifyAll();
    }

    [Fact]
    public async Task StoreAccountAuditEventAsync_ShouldUseSubjectAsActor()
    {
        // Given
        const string subjectUserId = "self-registering-user";
        UserEvent storedUserEvent = null;

        Mock<IUserEventService> serviceMock =
            new(behavior: MockBehavior.Strict);

        serviceMock
            .Setup(expression: service => service.AddUserEventAsync(
                userEvent: It.IsAny<UserEvent>()))
            .Callback<UserEvent>(action: userEvent =>
                storedUserEvent = userEvent)
            .ReturnsAsync(value: new UserEvent());

        AccountAuditUserEventProcessingService service = new(
            userEventService: serviceMock.Object,
            serializationBroker: new SerializationBroker());

        // When
        await service.StoreSecurityAccountEventAuditAsync(
            eventName:
                SecurityAccountEventKind.RegistrationCreated.ToEventName(),
            securityAccountEvent: new SecurityAccountEvent
            {
                Kind = SecurityAccountEventKind.RegistrationCreated,
                User = new SSOUser { Id = subjectUserId }
            });

        // Then
        storedUserEvent.CreatedBy.Should()
            .Be(expected: subjectUserId);

        serviceMock.VerifyAll();
    }
}