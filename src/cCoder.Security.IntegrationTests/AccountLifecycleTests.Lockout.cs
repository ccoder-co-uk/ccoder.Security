using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Xunit;

namespace cCoder.Security.IntegrationTests;

public partial class AccountLifecycleTests
{
    [Fact]
    public async Task ShouldRejectValidLoginAfterFailedAttemptsTriggerLockoutAsync()
    {
        // given
        RegisterUser user = CreateRegisterUser("lockout");
        (SSOUser registeredUser, string confirmationToken) = await RegisterAsync(user);
        await ConfirmRegistrationAsync(confirmationToken);

        Auth invalidAuth = CreateAuth(user, password: "WrongPass01!");

        // when
        for (int attempt = 0; attempt < 11; attempt++)
            await AssertLoginRejectedAsync(invalidAuth);

        // then
        FindUser(registeredUser.Id).LockoutEnabled.Should().BeTrue();
        await AssertLoginRejectedAsync(CreateAuth(user));
    }
}
