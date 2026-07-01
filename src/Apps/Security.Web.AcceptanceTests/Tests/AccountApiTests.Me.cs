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
        AccountApiClient accountClient = AccountApiClient.CreateUnauthenticated();

        accountClient.UseNoCookiesApiClient();

        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(existingRegisterUser);

        SSOUser existingSSOUser = result.User;

        Auth inputAuth = RandomAuth(existingRegisterUser);
        Token token = await accountClient.LoginAsync(inputAuth);

        //when
        accountClient.AddBearerAuthentication(token.Id);
        SSOUser actualSSOUser = await accountClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(existingSSOUser);

        await TearDownUserAsync(existingSSOUser.Id);
    }

    [Fact]
    public async Task MeWorksAsExpectedForSession()
    {
        //given
        AccountApiClient accountClient = AccountApiClient.CreateUnauthenticated();
        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(existingRegisterUser);

        SSOUser existingSSOUser = result.User;

        Auth inputAuth = RandomAuth(existingRegisterUser);
        Token token = await accountClient.LoginAsync(inputAuth);

        //when
        accountClient.AddBearerAuthentication(token.Id);
        SSOUser actualSSOUser = await accountClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(existingSSOUser);

        await TearDownUserAsync(existingSSOUser.Id);
    }

    [Fact]
    public async Task MeWorksAsExpectedForBasic()
    {
        //given
        AccountApiClient accountClient = AccountApiClient.CreateUnauthenticated();
        accountClient.UseNoCookiesApiClient();

        RegisterUser existingRegisterUser = RandomRegisterUser();

        RegistrationResult result = await registerApiClient
            .RegisterAsync(existingRegisterUser);

        SSOUser existingSSOUser = result.User;

        Auth inputAuth = RandomAuth(existingRegisterUser);

        //when
        accountClient.AddBasicAuthentication(inputAuth);
        SSOUser actualSSOUser = await accountClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(existingSSOUser);

        await TearDownUserAsync(existingSSOUser.Id);
    }
}
