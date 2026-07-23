// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Security.AcceptanceTests.Clients;
using Security.AcceptanceTests.Tests.Models;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class AccountApiTests
{
    [Fact]
    public async Task MeWorksAsExpectedForBearerToken()
    {
        //given
        using AccountApiClient accountClient = AccountApiClient.CreateUnauthenticated();

        accountClient.UseNoCookiesApiClient();

        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(registerUser: existingRegisterUser);

        SSOUser existingSSOUser = result.User;

        Auth inputAuth = RandomAuth(user: existingRegisterUser);
        Token token = await accountClient.LoginAsync(auth: inputAuth);

        //when
        accountClient.AddBearerAuthentication(bearer: token.Id);
        SSOUser actualSSOUser = await accountClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(expectation: existingSSOUser);

        await TearDownUserAsync(userId: existingSSOUser.Id);
    }

    [Fact]
    public async Task MeWorksAsExpectedForSession()
    {
        //given
        using AccountApiClient accountClient = AccountApiClient.CreateUnauthenticated();
        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(registerUser: existingRegisterUser);

        SSOUser existingSSOUser = result.User;

        Auth inputAuth = RandomAuth(user: existingRegisterUser);
        Token token = await accountClient.LoginAsync(auth: inputAuth);

        //when
        accountClient.AddBearerAuthentication(bearer: token.Id);
        SSOUser actualSSOUser = await accountClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(expectation: existingSSOUser);

        await TearDownUserAsync(userId: existingSSOUser.Id);
    }

    [Fact]
    public async Task MeWorksAsExpectedForBasic()
    {
        //given
        using AccountApiClient accountClient = AccountApiClient.CreateUnauthenticated();
        accountClient.UseNoCookiesApiClient();

        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(registerUser: existingRegisterUser);

        SSOUser existingSSOUser = result.User;

        Auth inputAuth = RandomAuth(user: existingRegisterUser);

        //when
        accountClient.AddBasicAuthentication(auth: inputAuth);
        SSOUser actualSSOUser = await accountClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(expectation: existingSSOUser);

        await TearDownUserAsync(userId: existingSSOUser.Id);
    }
}