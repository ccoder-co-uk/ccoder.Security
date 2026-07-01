using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Xunit;

namespace cCoder.Security.IntegrationTests;

public partial class AccountLifecycleTests
{
    [Fact]
    public async Task ShouldRegisterConfirmAndLoginAsync()
    {
        // given
        RegisterUser user = CreateRegisterUser("registration");

        // when
        (SSOUser registeredUser, string confirmationToken) = await RegisterAsync(user);
        await ConfirmRegistrationAsync(confirmationToken);
        Token token = await LoginAsync(CreateAuth(user));
        await LogoutAsync();

        // then
        registeredUser.Email.Should().Be(user.Email);
        token.UserName.Should().Be(registeredUser.Id);
        FindUser(registeredUser.Id).EmailConfirmed.Should().BeTrue();
    }
}
