using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Security.AcceptanceTests.Tests.Models;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class AccountApiTests
{
    [Fact]
    public async void LoginReturnsTokenAsync()
    {
        //given
        RegisterUser existingRegisterUser = RandomRegisterUser();
        RegistrationResult result = await registerApiClient.RegisterAsync(existingRegisterUser);

        Auth inputAuth = RandomAuth(existingRegisterUser);

        //when
        Token actualToken = await accountApiClient.LoginAsync(inputAuth);

        //then
        result.Token.Should().NotBeNullOrEmpty();
        actualToken.UserName.Should().BeEquivalentTo(result.User.Id);
        Assert.True(actualToken.Expires > DateTimeOffset.Now);
        Assert.True(actualToken.Reason == 0);

        await TearDownUserAsync(result.User.Id);
    }
}