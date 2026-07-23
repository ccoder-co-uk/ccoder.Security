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

public partial class RegistrationAggregationServiceTests
{
    [Fact]
    public async Task ShouldReturnExistingUserWithoutTokenWhenInviteEmailAlreadyExists()
    {
        // Given
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
            .Setup(expression: service => service.InviteSSOUserAsync(user:It.IsAny<SSOUser>()))
            .ThrowsAsync(exception: new ValidationException("Email exists"));

        ssoUserProcessingServiceMock
            .Setup(expression: service => service.GetAllSSOUsers(ignoreFilters:true))
            .Returns(value: new[] { existingUser }.AsQueryable());

        // When
        RegisterUser registration =
            await registrationAggregationService.InviteRegisterUserAsync(
                registerForm: input);

        // Then
        registration.User.Should()
            .BeSameAs(expected: existingUser);

        registration.User.PasswordHash.Should()
            .BeNull();

        registration.Token.Should()
            .BeNull();

        tokenProcessingServiceMock.Verify(
expression: service => service.GenerateInvitationToken(userId: It.IsAny<string>()),
times: Times.Never);

        accountEventProcessingServiceMock.Verify(
            expression: service => service.RaiseSecurityAccountEventRequestAsync(
accountEventRequest:                It.IsAny<SecurityAccountEventRequest>()),
            times: Times.Never);
    }
}