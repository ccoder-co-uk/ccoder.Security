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
    public async Task ConfirmEmailWorksAsExpected()
    {
        //given

        RegisterUser existingRegisterUser = RandomRegisterUser();

        Auth inputAuth = new()
        {
            User = existingRegisterUser.Email.Split(separator: "@")[0],
            Pass = existingRegisterUser.Password
        };

        //when
        RegistrationResult registrationResult =
            await userApiClient.RegisterAsync(registerUser: existingRegisterUser);

        SSOUser expectedSSOUser = registrationResult.User;
        expectedSSOUser.EmailConfirmed = true;

        await userApiClient
            .PostAsync(query: "ConfirmRegistration?confirmationToken=" + registrationResult.Token, content: null);

        Token loginToken = await accountApiClient.LoginAsync(auth: inputAuth);
        accountApiClient.AddBearerAuthentication(bearer: loginToken.Id);

        SSOUser actualSSOUser = await accountApiClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(expectation: expectedSSOUser);
        await TearDownUserAsync(userId: actualSSOUser.Id);
    }
}