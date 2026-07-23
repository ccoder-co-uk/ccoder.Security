// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Xunit;

namespace cCoder.Security.IntegrationTests;

public partial class AccountLifecycleTests
{
    [Fact]
    public async Task ShouldRejectValidLoginAfterFailedAttemptsTriggerLockoutAsync()
    {
        // given
        RegisterUser user = CreateRegisterUser(name: "lockout");
        (SSOUser registeredUser, string confirmationToken) = await RegisterAsync(user: user);
        await ConfirmRegistrationAsync(token: confirmationToken);

        Auth invalidAuth = CreateAuth(user: user, password: "WrongPass01!");

        // when
        for (int attempt = 0; attempt < 11; attempt++)
            await AssertLoginRejectedAsync(auth: invalidAuth);

        // then
        FindUser(userId: registeredUser.Id).LockoutEnabled.Should().BeTrue();
        await AssertLoginRejectedAsync(auth: CreateAuth(user));
    }
}