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
            User = existingRegisterUser.Email.Split("@")[0],
            Pass = existingRegisterUser.Password
        };

        //when
        RegistrationResult registrationResult = 
            await userApiClient.RegisterAsync(existingRegisterUser);

        SSOUser expectedSSOUser = registrationResult.User;
        expectedSSOUser.EmailConfirmed = true;

        await userApiClient
            .PostAsync("ConfirmRegistration?confirmationToken=" + registrationResult.Token, null);

        Token loginToken = await accountApiClient.LoginAsync(inputAuth);
        accountApiClient.AddBearerAuthentication(loginToken.Id);

        SSOUser actualSSOUser = await accountApiClient.Me();

        //then
        actualSSOUser.Should().BeEquivalentTo(expectedSSOUser);
        await TearDownUserAsync(actualSSOUser.Id);
    }
}
