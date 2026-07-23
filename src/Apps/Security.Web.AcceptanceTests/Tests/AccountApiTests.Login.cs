// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Security.AcceptanceTests.Tests.Models;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class AccountApiTests
{
    [Fact]
    public async Task LoginReturnsTokenAsync()
    {
        // Given
        RegisterUser existingRegisterUser = RandomRegisterUser();
        RegistrationResult result = await registerApiClient.RegisterAsync(registerUser: existingRegisterUser);

        Auth inputAuth = RandomAuth(user: existingRegisterUser);

        // When
        Token actualToken = await userApiClient.LoginAsync(auth: inputAuth);

        // Then
        result.Token.Should()
            .NotBeNullOrEmpty();

        actualToken.UserName.Should()
            .BeEquivalentTo(expected: result.User.Id);

        Assert.True(condition: actualToken.Expires > DateTimeOffset.Now);
        Assert.True(condition: actualToken.Reason == 0);

        await TearDownUserAsync(userId: result.User.Id);
    }
}