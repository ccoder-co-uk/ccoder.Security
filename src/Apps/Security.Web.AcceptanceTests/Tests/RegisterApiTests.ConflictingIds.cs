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
    public async Task ShouldRegisterMultipleAccountsWithSameEmailLocalPartAsync()
    {
        // Given.
        // Given
        RegisterUser inputRegisterUser1 = RandomRegisterUser();

        SSOUser expectedSSOUser1 = new()
        {
            AccessFailedCount = 0,
            DisplayName = inputRegisterUser1.DisplayName,
            Email = inputRegisterUser1.Email,
            EmailConfirmed = false,
            LockoutEnabled = false,
            LockoutEndDateUtc = null,
            PhoneNumber = inputRegisterUser1.PhoneNumber,
            PhoneNumberConfirmed = false,
            Id = inputRegisterUser1.Email.Split(separator: '@')[0]
        };

        RegisterUser inputRegisterUser2 = RandomRegisterUser();
        inputRegisterUser2.Email = inputRegisterUser1.Email + ".uk";

        SSOUser expectedSSOUser2 = new()
        {
            AccessFailedCount = 0,
            DisplayName = inputRegisterUser2.DisplayName,
            Email = inputRegisterUser1.Email + ".uk",
            EmailConfirmed = false,
            LockoutEnabled = false,
            LockoutEndDateUtc = null,
            PhoneNumber = inputRegisterUser2.PhoneNumber,
            PhoneNumberConfirmed = false,
            Id = expectedSSOUser1.Id + "1"
        };

        RegisterUser inputRegisterUser3 = RandomRegisterUser();
        inputRegisterUser3.Email = inputRegisterUser1.Email + ".com";

        SSOUser expectedSSOUser3 = new()
        {
            AccessFailedCount = 0,
            DisplayName = inputRegisterUser3.DisplayName,
            Email = inputRegisterUser1.Email + ".com",
            EmailConfirmed = false,
            LockoutEnabled = false,
            LockoutEndDateUtc = null,
            PhoneNumber = inputRegisterUser3.PhoneNumber,
            PhoneNumberConfirmed = false,
            Id = expectedSSOUser1.Id + "2"
        };

        // When.
        RegistrationResult result1 = await userApiClient
            .RegisterAsync(registerUser: inputRegisterUser1);

        RegistrationResult result2 = await userApiClient
            .RegisterAsync(registerUser: inputRegisterUser2);

        RegistrationResult result3 = await userApiClient
            .RegisterAsync(registerUser: inputRegisterUser3);

        SSOUser actualSSOUser1 = result1.User;
        SSOUser actualSSOUser2 = result2.User;
        SSOUser actualSSOUser3 = result3.User;

        expectedSSOUser1.PasswordHash = actualSSOUser1.PasswordHash;
        expectedSSOUser2.PasswordHash = actualSSOUser2.PasswordHash;
        // When
        expectedSSOUser3.PasswordHash = actualSSOUser3.PasswordHash;

        // Then.
        // Then
        actualSSOUser1.Should()
            .BeEquivalentTo(expectation: expectedSSOUser1);

        actualSSOUser2.Should()
            .BeEquivalentTo(expectation: expectedSSOUser2);

        actualSSOUser3.Should()
            .BeEquivalentTo(expectation: expectedSSOUser3);

        await TearDownUserAsync(userId: actualSSOUser1.Id);
        await TearDownUserAsync(userId: actualSSOUser2.Id);
        await TearDownUserAsync(userId: actualSSOUser3.Id);
    }
}