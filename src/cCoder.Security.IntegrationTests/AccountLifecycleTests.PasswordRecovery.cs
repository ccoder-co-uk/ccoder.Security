// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Entities;
using FluentAssertions;
using Xunit;

namespace cCoder.Security.IntegrationTests;

public partial class AccountLifecycleTests
{
    [Fact]
    public async Task ShouldRecoverLockedAccountAndAllowLoginAsync()
    {
        // Given
        RegisterUser user = CreateRegisterUser(name: "recovery");
        (SSOUser registeredUser, string confirmationToken) = await RegisterAsync(user: user);
        await ConfirmRegistrationAsync(token: confirmationToken);

        Auth invalidAuth = CreateAuth(user: user, password: "WrongPass01!");

        for (int attempt = 0; attempt < 11; attempt++)
        { await AssertLoginRejectedAsync(auth: invalidAuth); }

        FindUser(userId: registeredUser.Id)
            .LockoutEnabled.Should()
            .BeTrue();

        // When
        Token resetToken = await RequestPasswordResetTokenAsync(email: user.Email);

        await ConfirmForgotPasswordAsync(token: resetToken.Id, userId: registeredUser.Id, password: UpdatedPassword);
        Token loginToken = await LoginAsync(auth: CreateAuth(user: user, password: UpdatedPassword));

        // Then
        SSOUser recoveredUser = FindUser(userId: registeredUser.Id);

        recoveredUser.LockoutEnabled.Should()
            .BeFalse();

        recoveredUser.AccessFailedCount.Should()
            .Be(expected: 0);

        loginToken.UserName.Should()
            .Be(expected: registeredUser.Id);
    }
}