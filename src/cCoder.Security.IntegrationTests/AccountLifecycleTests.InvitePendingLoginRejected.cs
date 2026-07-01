using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Xunit;

namespace cCoder.Security.IntegrationTests;

public partial class AccountLifecycleTests
{
    [Fact]
    public async Task ShouldRejectLoginForInviteThatHasNotBeenAcceptedAsync()
    {
        // given
        RegisterUser user = CreateRegisterUser("pending-invite");
        user.Password = null;

        // when
        (SSOUser invitedUser, string inviteToken) = await InviteAsync(user);
        string resentInviteToken = await ResendInviteAsync(invitedUser.Id);

        user.Password = DefaultPassword;

        // then
        inviteToken.Should().NotBeNullOrWhiteSpace();
        resentInviteToken.Should().NotBeNullOrWhiteSpace();
        FindUser(invitedUser.Id).LockoutEnabled.Should().BeTrue();

        await AssertLoginRejectedAsync(CreateAuth(user));
    }
}
