using FluentAssertions;
using Security.AcceptanceTests.Tests.Models;
using Security.Objects.DTOs;
using Security.Objects.Entities;
using System;
using Xunit;

namespace Security.AcceptanceTests.Tests
{
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
}