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
    public async Task ShouldRegisterConfirmAndLoginAsync()
    {
        // Given
        RegisterUser user = CreateRegisterUser(name: "registration");

        // When
        (SSOUser registeredUser, string confirmationToken) = await RegisterAsync(user: user);
        await ConfirmRegistrationAsync(token: confirmationToken);
        Token token = await LoginAsync(auth: CreateAuth(user: user));

        // Then
        registeredUser.Email.Should()
            .Be(expected: user.Email);

        token.UserName.Should()
            .Be(expected: registeredUser.Id);

        SSOUser storedUser = FindUser(userId: registeredUser.Id);

        storedUser.EmailConfirmed.Should()
            .BeTrue();

        storedUser.PasswordHash.Should()
            .StartWith(expected: "$argon2id$v=19$m=19456,t=2,p=1$");

        storedUser.PasswordHash.Should()
            .NotContain(unexpected: user.Password);

        string tokenSelector = token.Id.Split(separator: '.')[0];
        Token storedToken = FindStoredToken(selector: tokenSelector);

        storedToken.Id.Should()
            .NotBe(unexpected: token.Id);

        storedToken.SecretHash.Should()
            .NotBeNullOrWhiteSpace();

        await LogoutAsync();
    }
}