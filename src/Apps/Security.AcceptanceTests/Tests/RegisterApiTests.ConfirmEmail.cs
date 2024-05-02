using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Security.AcceptanceTests.Tests.Models;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class RegisterApiTests
{
    [Fact]
    public async void ConfirmEmailWorksAsExpected()
    {
        //given

        RegisterUser existingRegisterUser = RandomRegisterUser();

        Auth inputAuth = new()
        {
            User = existingRegisterUser.Email.Split("@")[0],
            Pass = existingRegisterUser.Password
        };

        //when
        RegistrationResult registrationResult = 
            await registerApiClient.RegisterAsync(existingRegisterUser);

        SSOUser expectedSSOUser = registrationResult.User;
        expectedSSOUser.EmailConfirmed = true;

        await registerApiClient
            .PostAsync("ConfirmRegistration?confirmationToken=" + registrationResult.Token, null);

        Token loginToken = await accountApiClient.LoginAsync(inputAuth);
        ssoUserApiClient.AddBearerAuthentication(loginToken.Id);

        SSOUser actualSSOUser = await ssoUserApiClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(expectedSSOUser);
        await TearDownUserAsync(actualSSOUser.Id);
    }
}