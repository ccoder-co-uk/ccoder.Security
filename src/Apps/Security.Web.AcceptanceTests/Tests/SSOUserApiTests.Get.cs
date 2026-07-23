// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Security.AcceptanceTests.Tests.Models;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class SSOUserApiTests
{
    [Fact]
    public async Task ShouldGetAllSSOUsersAsync()
    {
        //given
        RegisterUser[] randomUsers = RandomRegisterUsers();
        List<SSOUser> expectedSSOUsers = [];

        foreach (RegisterUser registerUser in randomUsers)
        {
            RegistrationResult result = await registerApiClient.RegisterAsync(registerUser: registerUser);

            expectedSSOUsers.Add(item: result.User);
        }

        //when
        IEnumerable<SSOUser> actualSSOUsers = await ssoUserApiClient.GetAllSSOUsersAsync();

        //then
        foreach (SSOUser expectedUser in expectedSSOUsers)
        {
            SSOUser actualUser = actualSSOUsers.FirstOrDefault(predicate: u => u.Id == expectedUser.Id);
            actualUser.Should().BeEquivalentTo(expectation: expectedUser);
        }

        foreach (SSOUser ssoUser in expectedSSOUsers)
            await registerApiClient.TearDown(ssoUserId: ssoUser.Id);
    }
}