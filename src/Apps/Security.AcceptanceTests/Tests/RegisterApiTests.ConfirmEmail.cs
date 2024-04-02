using FluentAssertions;
using Security.AcceptanceTests.Tests.Models;
using Security.Objects.DTOs;
using Security.Objects.Entities;
using Xunit;

namespace Security.AcceptanceTests.Tests
{
    public partial class RegisterApiTests
	{
        [Fact]
        public async void ConfirmEmailWorksAsExpected()
        {
            //given

            RegisterUser existingRegisterUser = RandomRegisterUser();

            Auth inputAuth = new Auth
            {
                User = existingRegisterUser.Email.Split("@")[0],
                Pass = existingRegisterUser.Password
            };

            //when
            RegistrationResult registrationResult = 
                await registerApiClient.RegisterAsync(existingRegisterUser);

            SSOUser expectedSSOUser = registrationResult.User;
            expectedSSOUser.EmailConfirmed = true;
            expectedSSOUser.EmailConfirmed = true;

            await registerApiClient
                .PostAsync("ConfirmRegistration?confirmationToken=" + registrationResult.Token, null);

            var loginToken = await accountApiClient.LoginAsync(inputAuth);
            ssoUserApiClient.AddBearerAuthentication(loginToken.Id);

            SSOUser actualSSOUser = await ssoUserApiClient.Me();

            //then
            actualSSOUser.Should().BeEquivalentTo(expectedSSOUser);
            await TearDownUserAsync(actualSSOUser.Id);
        }
    }
}

