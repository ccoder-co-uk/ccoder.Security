using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Security.AcceptanceTests.Tests.Models;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class RegisterApiTests
{
    [Fact]
    public async Task ShouldRegisterAccountAsync()
    {
        //given
        RegisterUser inputRegisterUser = RandomRegisterUser();
        SSOUser expectedSSOUser = new()
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
        RegistrationResult result = await userApiClient
            .RegisterAsync(inputRegisterUser);

        SSOUser actualSSOUser = result.User;    

        expectedSSOUser.Id = actualSSOUser.Id;
        expectedSSOUser.PasswordHash = actualSSOUser.PasswordHash;

        //then
        actualSSOUser.Should().BeEquivalentTo(expectedSSOUser);
        await TearDownUserAsync(actualSSOUser.Id);
    }
}

