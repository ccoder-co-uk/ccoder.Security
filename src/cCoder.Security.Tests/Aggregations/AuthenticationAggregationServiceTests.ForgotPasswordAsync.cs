// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.Events;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Aggregations;

public partial class AuthenticationAggregationServiceTests
{
    [Fact]
    public async Task ShouldGenerateForgotPasswordTokenAndRaisePasswordResetRequestedEvent()
    {
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
            .Setup(expression: service => service.GetAllSSOUsers(true))
            .Returns(value: new[] { user }.AsQueryable());

        tokenProcessingServiceMock
            .Setup(expression: service => service.GenerateForgottenPasswordToken(user.Id))
            .ReturnsAsync(value: token);

        accountEventProcessingServiceMock
            .Setup(expression: service => service.RaiseSecurityAccountEventRequestAsync(
                It.Is<SecurityAccountEventRequest>(request =>
                    request.Kind == SecurityAccountEventKind.PasswordResetRequested
                    && request.User == user
                    && request.Token == token.Id)))
            .Returns(value: ValueTask.CompletedTask);

        Token actualToken =
            await authenticationAggregationService.ForgotPasswordAsync(
                email: user.Email);

        actualToken.Should().BeSameAs(expected: token);

        accountEventProcessingServiceMock.Verify(
            expression: service => service.RaiseSecurityAccountEventRequestAsync(
                It.Is<SecurityAccountEventRequest>(request =>
                    request.Kind == SecurityAccountEventKind.PasswordResetRequested
                    && request.User == user
                    && request.Token == token.Id)),
            times: Times.Once);
    }
}