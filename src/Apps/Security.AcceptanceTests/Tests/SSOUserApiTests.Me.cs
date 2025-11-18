using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Security.AcceptanceTests.Clients;
using Security.AcceptanceTests.Tests.Models;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class SSOUserApiTests
{
    [Fact]
    public async Task MeWorksAsExpectedForBearerToken()
    {
        //given
        AccountApiClient accountClient = new();
        SSOUserApiClient ssoUserClient = new();

        accountClient.UseNoCookiesApiClient();

        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(existingRegisterUser);

        SSOUser existingSSOUser = result.User;

        Auth inputAuth = RandomAuth(existingRegisterUser);
        Token token = await accountClient.LoginAsync(inputAuth);

        //when
        ssoUserClient.AddBearerAuthentication(token.Id);
        SSOUser actualSSOUser = await ssoUserClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(existingSSOUser);

        await TearDownUserAsync(existingSSOUser.Id);
    }

    [Fact]
    public async Task MeWorksAsExpectedForSession()
    {
        //given
        CookieSSOUserApiClient cookieSsoUserClient = new();
        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(existingRegisterUser);

        SSOUser existingSSOUser = result.User;  

        Auth inputAuth = RandomAuth(existingRegisterUser);
        Token token = await cookieSsoUserClient.LoginAsync(inputAuth, keepSessionCookie: true);

        //when
        SSOUser actualSSOUser = await cookieSsoUserClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(existingSSOUser);

        await TearDownUserAsync(existingSSOUser.Id);
    }

    [Fact]
    public async Task MeWorksAsExpectedForBasic()
    {
        //given
        AccountApiClient accountClient = new();
        SSOUserApiClient ssoUserClient = new();
        accountClient.UseNoCookiesApiClient();

        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(existingRegisterUser);

        SSOUser existingSSOUser = result.User;

        Auth inputAuth = RandomAuth(existingRegisterUser);

        //when
        ssoUserClient.AddBasicAuthentication(inputAuth);
        SSOUser actualSSOUser = await ssoUserClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(existingSSOUser);

        await TearDownUserAsync(existingSSOUser.Id);
    }
}
