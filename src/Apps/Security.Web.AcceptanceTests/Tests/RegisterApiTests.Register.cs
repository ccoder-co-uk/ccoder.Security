// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Security.AcceptanceTests.Tests.Models;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class RegisterApiTests
{
    [Fact]
    public async Task ShouldRegisterAccountAsync()
    {
        // Given
        RegisterUser inputRegisterUser = RandomRegisterUser();

        SSOUser expectedSSOUser = new()
        {
            AccessFailedCount = 0,
            DisplayName = inputRegisterUser.DisplayName,
            Email = inputRegisterUser.Email,
            EmailConfirmed = false,
            LockoutEnabled = false,
            LockoutEndDateUtc = null,
            PhoneNumber = inputRegisterUser.PhoneNumber,
            PhoneNumberConfirmed = false,
        };

        // When
        RegistrationResult result = await userApiClient
            .RegisterAsync(registerUser: inputRegisterUser);

        SSOUser actualSSOUser = result.User;

        expectedSSOUser.Id = actualSSOUser.Id;
        expectedSSOUser.PasswordHash = actualSSOUser.PasswordHash;

        // Then
        actualSSOUser.Should()
            .BeEquivalentTo(expectation: expectedSSOUser);

        await TearDownUserAsync(userId: actualSSOUser.Id);
    }
}