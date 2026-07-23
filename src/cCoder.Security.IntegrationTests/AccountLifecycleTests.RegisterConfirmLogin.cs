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
    public async Task ShouldRegisterConfirmAndLoginAsync()
    {
        // Given
        RegisterUser user = CreateRegisterUser(name: "registration");

        // When
        (SSOUser registeredUser, string confirmationToken) = await RegisterAsync(user: user);
        await ConfirmRegistrationAsync(token: confirmationToken);
        Token token = await LoginAsync(auth: CreateAuth(user: user));
        await LogoutAsync();

        // Then
        registeredUser.Email.Should()
            .Be(expected: user.Email);

        token.UserName.Should()
            .Be(expected: registeredUser.Id);

        FindUser(userId: registeredUser.Id)
            .EmailConfirmed.Should()
            .BeTrue();
    }
}