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
        public async void ShouldRegisterAccountAsync()
        {
            //given
            RegisterUser inputRegisterUser = RandomRegisterUser();
            SSOUser expectedSSOUser = new SSOUser()
            {
                AccessFailedCount = 0,
                DisplayName = inputRegisterUser.DisplayName,
                Email = inputRegisterUser.Email,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = null,
                PhoneNumber = inputRegisterUser.PhoneNumber,
                PhoneNumberConfirmed = false,
            };

            //when
            RegistrationResult result = await registerApiClient
                .RegisterAsync(inputRegisterUser);

            SSOUser actualSSOUser = result.User;    

            expectedSSOUser.Id = actualSSOUser.Id;
            expectedSSOUser.PasswordHash = actualSSOUser.PasswordHash;

            //then
            actualSSOUser.Should().BeEquivalentTo(expectedSSOUser);
            await TearDownUserAsync(actualSSOUser.Id);
        }
    }
}
