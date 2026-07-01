using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Xunit;

namespace cCoder.Security.IntegrationTests;

public partial class AccountLifecycleTests
{
    [Fact]
    public async Task ShouldInviteAcceptAndLoginAsync()
    {
        // given
        RegisterUser user = CreateRegisterUser("accepted-invite");
        user.Password = null;

        // when
        (SSOUser invitedUser, string inviteToken) = await InviteAsync(user);

        user.Password = DefaultPassword;

        await AcceptInviteAsync(invitedUser.Id, inviteToken, user);
        Token token = await LoginAsync(CreateAuth(user));

        // then
        token.UserName.Should().Be(invitedUser.Id);
        FindUser(invitedUser.Id).LockoutEnabled.Should().BeFalse();
    }
}
