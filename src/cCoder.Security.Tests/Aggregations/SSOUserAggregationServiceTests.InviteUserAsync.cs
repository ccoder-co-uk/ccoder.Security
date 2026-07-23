// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.Events;
using FluentAssertions;
using Moq;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace cCoder.Security.Tests.Aggregations;

public partial class SSOUserAggregationServiceTests
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
            .Setup(expression: service => service.InviteSSOUserAsync(It.IsAny<SSOUser>()))
            .ThrowsAsync(exception: new ValidationException("Email exists"));

        ssoUserProcessingServiceMock
            .Setup(expression: service => service.GetAllSSOUsers(true))
            .Returns(value: new[] { existingUser }.AsQueryable());

        (SSOUser actualUser, string token) =
            await ssoUserAggregationService.InviteRegisterUserAsync(
                registerForm: input);

        actualUser.Should().BeSameAs(expected: existingUser);
        actualUser.PasswordHash.Should().BeNull();
        token.Should().BeNull();

        tokenProcessingServiceMock.Verify(
expression: service => service.GenerateInvitationToken(userId: It.IsAny<string>()),
times: Times.Never);

        accountEventProcessingServiceMock.Verify(
            expression: service => service.RaiseSecurityAccountEventRequestAsync(
                It.IsAny<SecurityAccountEventRequest>()),
            times: Times.Never);
    }
}