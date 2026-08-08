// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations;
using cCoder.Security.Services.Processings.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Services;

public sealed partial class ServiceSuccessCoverageTests
{
    [Fact]
    public async Task ShouldCompletePasswordResetAndRegistrationTokenFlows()
    {
        // Given

        const string userId = "coverage-user";
        const string password = "CoveragePassword123!";
        SSOUser user = new() { Id = userId };

        Token forgottenPasswordToken = new()
        {
            Id = "forgotten-token",
            UserName = userId
        };

        Token invitationToken = new()
        {
            Id = "invitation-token",
            UserName = userId
        };

        Token confirmationToken = new()
        {
            Id = "confirmation-token",
            UserName = userId
        };

        RegisterUser registration = new()
        {
            Email = "coverage@example.test",
            DisplayName = "Coverage User",
            Password = password
        };

        Mock<ISSOUserProcessingService> users = new();
        Mock<ITokenProcessingService> tokens = new();
        Mock<ISessionProcessingService> sessions = new();
        Mock<IAccountEventProcessingService> events = new();
        Mock<ILoggingProcessingService> logging = new();

        users
            .Setup(expression: service => service.FindById(ssoUserId: userId))
            .Returns(value: user);

        users
            .Setup(expression: service =>
                service.UpdateSSOUserAsync(item: It.IsAny<SSOUser>()))
            .ReturnsAsync(value: user);

        tokens
            .Setup(expression: service =>
                service.GetForgottenPasswordToken(
                    tokenId: forgottenPasswordToken.Id))
            .Returns(value: forgottenPasswordToken);

        tokens
            .Setup(expression: service =>
                service.GetInvitationToken(tokenId: invitationToken.Id))
            .Returns(value: invitationToken);

        tokens
            .Setup(expression: service =>
                service.GetConfirmationToken(tokenId: confirmationToken.Id))
            .Returns(value: confirmationToken);

        AuthenticationAggregationService authentication = new(
            ssoUserProcessingService: users.Object,
            tokenProcessingService: tokens.Object,
            sessionProcessingService: sessions.Object,
            accountEventProcessingService: events.Object,
            loggingProcessingService: logging.Object);

        Mock<ITenantProcessingService> tenants = new();
        Mock<ISSORoleProcessingService> roles = new();
        Mock<ISSOUserRoleProcessingService> userRoles = new();
        Mock<IAuthorizationProcessingService> authorization = new();

        RegistrationAggregationService registrations = new(
            ssoUserProcessingService: users.Object,
            tenantProcessingService: tenants.Object,
            tokenProcessingService: tokens.Object,
            roleProcessingService: roles.Object,
            userRoleProcessingService: userRoles.Object,
            accountEventProcessingService: events.Object,
            authorizationProcessingService: authorization.Object,
            loggingProcessingService: logging.Object);

        // When

        await authentication.ConfirmForgotPasswordAsync(
            tokenId: forgottenPasswordToken.Id,
            userId: userId,
            newPassword: password,
            confirmNewPassword: password);

        RegisterUser accepted = await registrations.AcceptRegisterUserInviteAsync(
            registerForm: registration,
            userId: userId,
            tokenId: invitationToken.Id);

        await registrations.ConfirmRegistration(
            tokenId: confirmationToken.Id);

        // Then

        accepted
            .Should()
            .BeSameAs(expected: registration);

        accepted.User
            .Should()
            .BeSameAs(expected: user);

        user.EmailConfirmed
            .Should()
            .BeTrue();

        tokens.Verify(
            expression: service =>
                service.DeleteTokenAsync(tokenId: It.IsAny<string>()),
            times: Times.Exactly(callCount: 3));

        users.Verify(
            expression: service =>
                service.UpdateSSOUserAsync(item: It.IsAny<SSOUser>()),
            times: Times.Exactly(callCount: 3));
    }
}