// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Orchestrations;

public partial class AuthenticationOrchestrationServiceTests
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
            .Setup(service => service.GetAllSSOUsers(true))
            .Returns(value: new[] { user }.AsQueryable());
        tokenProcessingServiceMock
            .Setup(service => service.GenerateForgottenPasswordToken(user.Id))
            .ReturnsAsync(value: token);
        accountEventServiceMock
            .Setup(service => service.RaisePasswordResetRequestedEventAsync(user, token.Id))
            .Returns(value: ValueTask.CompletedTask);

        Token actualToken =
            await authenticationOrchestrationService.ForgotPasswordAsync(email: user.Email);

        actualToken.Should().BeSameAs(expected: token);
        accountEventServiceMock.Verify(
expression: service => service.RaisePasswordResetRequestedEventAsync(user, token.Id),
times: Times.Once);
    }
}