// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System;
using System.Threading.Tasks;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Events;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Aggregations;

public partial class AuthenticationAggregationServiceTests
{
    [Fact]
    public async Task ShouldNotDiscloseUnknownForgotPasswordAccount()
    {
        // Given
        string unknownEmail = "unknown.user@example.com";

        ssoUserProcessingServiceMock
            .Setup(expression: service => service.GetAllSSOUsers(ignoreFilters: true))
            .Returns(value: Array.Empty<SSOUser>()
                .AsQueryable());

        // When
        Token actualToken =
            await authenticationAggregationService.ForgotPasswordAsync(
                email: unknownEmail);

        // Then
        actualToken
            .Should()
            .BeNull();

        loggingProcessingServiceMock.Verify(
            expression: service => service.LogWarning(
                message: $"Password reset account not found: {unknownEmail}"),
            times: Times.Once);

        tokenProcessingServiceMock.Verify(
            expression: service => service.GenerateForgottenPasswordToken(
                userId: It.IsAny<string>()),
            times: Times.Never);

        accountEventProcessingServiceMock.Verify(
            expression: service => service.RaiseSecurityAccountEventRequestAsync(
                accountEventRequest: It.IsAny<SecurityAccountEventRequest>()),
            times: Times.Never);
    }

    [Fact]
    public async Task ShouldGenerateForgotPasswordTokenAndRaisePasswordResetRequestedEvent()
    {
        // Given
        SSOUser user = new()
        {
            Id = "existing.user",
            Email = "existing.user@example.com"
        };

        Token token = new()
        {
            Id = "forgot-password-token",
            UserName = user.Id
        };

        ssoUserProcessingServiceMock
            .Setup(expression: service => service.GetAllSSOUsers(ignoreFilters:true))
            .Returns(value: new[] { user }.AsQueryable());

        tokenProcessingServiceMock
            .Setup(expression: service => service.GenerateForgottenPasswordToken(userId:user.Id))
            .ReturnsAsync(value: token);

        accountEventProcessingServiceMock
            .Setup(expression: service => service.RaiseSecurityAccountEventRequestAsync(
accountEventRequest:                It.Is<SecurityAccountEventRequest>(match:request =>
                    request.Kind == SecurityAccountEventKind.PasswordResetRequested
                    && request.User == user
                    && request.Token == token.Id)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Token actualToken =
            await authenticationAggregationService.ForgotPasswordAsync(
                email: user.Email);

        // Then
        actualToken.Should()
            .BeSameAs(expected: token);

        accountEventProcessingServiceMock.Verify(
            expression: service => service.RaiseSecurityAccountEventRequestAsync(
accountEventRequest:                It.Is<SecurityAccountEventRequest>(match:request =>
                    request.Kind == SecurityAccountEventKind.PasswordResetRequested
                    && request.User == user
                    && request.Token == token.Id)),
            times: Times.Once);
    }
}