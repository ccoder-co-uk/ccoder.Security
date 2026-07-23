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
    public async Task ShouldRejectLoginForInviteThatHasNotBeenAcceptedAsync()
    {
        // given
        RegisterUser user = CreateRegisterUser(name: "pending-invite");
        user.Password = null;

        // when
        (SSOUser invitedUser, string inviteToken) = await InviteAsync(user: user);
        string resentInviteToken = await ResendInviteAsync(userId: invitedUser.Id);

        user.Password = DefaultPassword;

        // then
        inviteToken.Should().NotBeNullOrWhiteSpace();
        resentInviteToken.Should().NotBeNullOrWhiteSpace();
        FindUser(userId: invitedUser.Id).LockoutEnabled.Should().BeTrue();

        await AssertLoginRejectedAsync(auth: CreateAuth(user: user));
    }
}