// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using System.Security;
using Xunit;

namespace cCoder.Security.Tests.Processings;

public partial class SSOUserProcessingServiceTests
{
    [Fact]
    public async Task FindByUserAndPasswordWorksAsExpected()
    {
        //given
        string inputPassword = RandomString();

        IQueryable<SSOUser> ssoUsersInService = RandomSSOUsers()
            .AsQueryable();

        foreach (SSOUser user in ssoUsersInService)
        { user.LockoutEnabled = false; }

        ssoUserServiceMock.Setup(expression: ssoUserServiceMock =>
            ssoUserServiceMock.GetAllSSOUsers(true))
            .Returns(value: ssoUsersInService);

        SSOUser expectedSSOUser = ssoUsersInService.First();

        passwordEncryptionBrokerMock.Setup(expression: passwordEncryptionBrokerMock =>
                passwordEncryptionBrokerMock.EncryptedAndPlainTextAreEqual(expectedSSOUser.PasswordHash, inputPassword))
                .Returns(value: true);

        //when
        SSOUser actualSSOUser = await ssoUserProcessingService
            .FindByUserAndPasswordAsync(username: expectedSSOUser.Id, password: inputPassword);

        //then
        actualSSOUser.Should().BeEquivalentTo(expectation: expectedSSOUser);

        ssoUserServiceMock.Verify(expression: ssoUserServiceMock =>
            ssoUserServiceMock.GetAllSSOUsers(ignoreFilters: true),
times: Times.Exactly(callCount: 2));

        passwordEncryptionBrokerMock.Verify(expression: passwordEncryptionBrokerMock =>
            passwordEncryptionBrokerMock.EncryptedAndPlainTextAreEqual(encrypted: expectedSSOUser.PasswordHash, plainText: inputPassword),
times: Times.Once);
    }

    [Fact]
    public async Task FindByUserAndPasswordNotWorksForLockoutAsExpected()
    {
        //given
        string inputPassword = RandomString();

        IQueryable<SSOUser> ssoUsersInService = RandomSSOUsers()
            .AsQueryable();

        ssoUserServiceMock.Setup(expression: ssoUserServiceMock =>
            ssoUserServiceMock.GetAllSSOUsers(true))
            .Returns(value: ssoUsersInService);

        SSOUser expectedSSOUser = ssoUsersInService.First();

        expectedSSOUser.LockoutEnabled = true;

        passwordEncryptionBrokerMock.Setup(expression: passwordEncryptionBrokerMock =>
            passwordEncryptionBrokerMock.EncryptedAndPlainTextAreEqual(expectedSSOUser.PasswordHash, inputPassword))
            .Returns(value: true);

        //when & then
        await Assert.ThrowsAsync<SecurityException>(testCode: async () => await ssoUserProcessingService
            .FindByUserAndPasswordAsync(username: expectedSSOUser.Id, password: inputPassword));
    }
}