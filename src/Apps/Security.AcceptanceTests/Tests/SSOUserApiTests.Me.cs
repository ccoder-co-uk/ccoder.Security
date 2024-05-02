using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Security.AcceptanceTests.Tests.Models;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class SSOUserApiTests
{
    [Fact]
    public async void MeWorksAsExpectedForBearerToken()
    {
        //given
        accountApiClient.UseNoCookiesApiClient();

        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(existingRegisterUser);

        SSOUser existingSSOUser = result.User;

        Auth inputAuth = RandomAuth(existingRegisterUser);
        Token token = await accountApiClient.LoginAsync(inputAuth);

        //when
        ssoUserApiClient.AddBearerAuthentication(token.Id);
        SSOUser actualSSOUser = await ssoUserApiClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(existingSSOUser);

        await TearDownUserAsync(existingSSOUser.Id);
    }

    [Fact]
    public async void MeWorksAsExpectedForSession()
    {
        //given
        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(existingRegisterUser);

        SSOUser existingSSOUser = result.User;  

        Auth inputAuth = RandomAuth(existingRegisterUser);
        Token token = await cookieSSOUserApiClient.LoginAsync(inputAuth, keepSessionCookie: true);

        //when
        SSOUser actualSSOUser = await cookieSSOUserApiClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(existingSSOUser);

        await TearDownUserAsync(existingSSOUser.Id);
    }

    [Fact]
    public async void MeWorksAsExpectedForBasic()
    {
        //given
        accountApiClient.UseNoCookiesApiClient();

        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(existingRegisterUser);

        SSOUser existingSSOUser = result.User;

        Auth inputAuth = RandomAuth(existingRegisterUser);

        //when
        ssoUserApiClient.AddBasicAuthentication(inputAuth);
        SSOUser actualSSOUser = await ssoUserApiClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(existingSSOUser);

        await TearDownUserAsync(existingSSOUser.Id);
    }
}
