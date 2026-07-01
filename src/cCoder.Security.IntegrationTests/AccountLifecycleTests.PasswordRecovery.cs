using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Xunit;

namespace cCoder.Security.IntegrationTests;

public partial class AccountLifecycleTests
{
    [Fact]
    public async Task ShouldRecoverLockedAccountAndAllowLoginAsync()
    {
        // given
        RegisterUser user = CreateRegisterUser("recovery");
        (SSOUser registeredUser, string confirmationToken) = await RegisterAsync(user);
        await ConfirmRegistrationAsync(confirmationToken);

        Auth invalidAuth = CreateAuth(user, password: "WrongPass01!");

        for (int attempt = 0; attempt < 11; attempt++)
            await AssertLoginRejectedAsync(invalidAuth);

        FindUser(registeredUser.Id).LockoutEnabled.Should().BeTrue();

        // when
        await RequestPasswordResetAsync(user.Email);

        Token resetToken = FindToken(registeredUser.Id, TokenUse.PasswordReset);

        await ConfirmForgotPasswordAsync(resetToken.Id, registeredUser.Id, UpdatedPassword);
        Token loginToken = await LoginAsync(CreateAuth(user, UpdatedPassword));

        // then
        SSOUser recoveredUser = FindUser(registeredUser.Id);
        recoveredUser.LockoutEnabled.Should().BeFalse();
        recoveredUser.AccessFailedCount.Should().Be(0);
        loginToken.UserName.Should().Be(registeredUser.Id);
    }
}
