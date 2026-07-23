// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        RegisterUser user = CreateRegisterUser(name: "accepted-invite");
        user.Password = null;

        // when
        (SSOUser invitedUser, string inviteToken) = await InviteAsync(user: user);

        user.Password = DefaultPassword;

        await AcceptInviteAsync(userId: invitedUser.Id, token: inviteToken, user: user);
        Token token = await LoginAsync(auth: CreateAuth(user));

        // then
        token.UserName.Should().Be(expected: invitedUser.Id);
        FindUser(userId: invitedUser.Id).LockoutEnabled.Should().BeFalse();
    }
}