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
        // Given
        RegisterUser user = CreateRegisterUser(name: "accepted-invite");
        user.Password = null;

        // When
        (SSOUser invitedUser, string inviteToken) = await InviteAsync(user: user);

        user.Password = DefaultPassword;

        await AcceptInviteAsync(userId: invitedUser.Id, token: inviteToken, user: user);
        Token token = await LoginAsync(auth: CreateAuth(user: user));

        // Then
        token.UserName.Should()
            .Be(expected: invitedUser.Id);

        FindUser(userId: invitedUser.Id)
            .LockoutEnabled.Should()
            .BeFalse();
    }
}