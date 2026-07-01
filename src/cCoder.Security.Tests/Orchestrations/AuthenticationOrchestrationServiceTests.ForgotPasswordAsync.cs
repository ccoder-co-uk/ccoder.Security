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
            .Returns(new[] { user }.AsQueryable());
        tokenProcessingServiceMock
            .Setup(service => service.GenerateForgottenPasswordToken(user.Id))
            .ReturnsAsync(token);
        accountEventServiceMock
            .Setup(service => service.RaisePasswordResetRequestedEventAsync(user, token.Id))
            .Returns(ValueTask.CompletedTask);

        Token actualToken =
            await authenticationOrchestrationService.ForgotPasswordAsync(user.Email);

        actualToken.Should().BeSameAs(token);
        accountEventServiceMock.Verify(
            service => service.RaisePasswordResetRequestedEventAsync(user, token.Id),
            Times.Once);
    }
}
