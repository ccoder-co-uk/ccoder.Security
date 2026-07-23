// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace cCoder.Security.Tests.Orchestrations;

public partial class SSOUserOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReturnExistingUserWithoutTokenWhenInviteEmailAlreadyExists()
    {
        RegisterUser input = new()
        {
            DisplayName = "Existing User",
            Email = "existing.user@example.com",
            PhoneNumber = "123"
        };

        SSOUser existingUser = new()
        {
            Id = "existing.user",
            DisplayName = input.DisplayName,
            Email = input.Email,
            PasswordHash = "secret"
        };

        ssoUserProcessingServiceMock
            .Setup(service => service.InviteSSOUserAsync(It.IsAny<SSOUser>()))
            .ThrowsAsync(exception: new ValidationException("Email exists"));

        ssoUserProcessingServiceMock
            .Setup(service => service.GetAllSSOUsers(true))
            .Returns(value: new[] { existingUser }.AsQueryable());

        (SSOUser actualUser, string token) =
            await ssoUserOrchestrationService.InviteUserAsync(registerForm: input);

        actualUser.Should().BeSameAs(expected: existingUser);
        actualUser.PasswordHash.Should().BeNull();
        token.Should().BeNull();

        tokenProcessingServiceMock.Verify(
expression: service => service.GenerateInvitationToken(It.IsAny<string>()),
times: Times.Never);

        accountEventServiceMock.Verify(
expression: service => service.RaiseInvitationCreatedEventAsync(
                It.IsAny<SSOUser>(),
                It.IsAny<RegisterUser>(),
                It.IsAny<string>()),
times: Times.Never);
    }
}