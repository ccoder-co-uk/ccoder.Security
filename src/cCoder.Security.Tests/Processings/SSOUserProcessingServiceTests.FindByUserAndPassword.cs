// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Models;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings;

#pragma warning disable STXFORMAT008 // Long utility-broker verification expressions trigger a formatting false positive.
public partial class SSOUserProcessingServiceTests
{
    [Fact]
    public async Task FindByUserAndPasswordWorksAsExpected()
    {
        // Given
        string inputPassword = RandomString();

        IQueryable<SSOUser> ssoUsersInService = RandomSSOUsers()
            .AsQueryable();

        foreach (SSOUser user in ssoUsersInService)
        { user.LockoutEnabled = false; }

        ssoUserServiceMock.Setup(expression: ssoUserServiceMock =>
            ssoUserServiceMock.GetAllSSOUsers(ignoreFilters: true))
            .Returns(value: ssoUsersInService);

        SSOUser expectedSSOUser = ssoUsersInService.First();

        passwordHashingBrokerMock.Setup(expression: passwordHashingBrokerMock =>
                passwordHashingBrokerMock.VerifyHashedPassword(
                    hashedPassword: expectedSSOUser.PasswordHash,
                    providedPassword: inputPassword))
                .Returns(value: PasswordVerificationOutcome.Success);

        // When
        SSOUser actualSSOUser = await ssoUserProcessingService
            .FindByUserAndPasswordAsync(username: expectedSSOUser.Id, password: inputPassword);

        // Then
        actualSSOUser.Should()
            .BeEquivalentTo(expectation: expectedSSOUser);

        ssoUserServiceMock.Verify(expression: ssoUserServiceMock =>
            ssoUserServiceMock.GetAllSSOUsers(ignoreFilters: true),
times: Times.Exactly(callCount: 2));

        passwordHashingBrokerMock.Verify(expression: passwordHashingBrokerMock =>
            passwordHashingBrokerMock.VerifyHashedPassword(
                hashedPassword: expectedSSOUser.PasswordHash,
                providedPassword: inputPassword),
times: Times.Once);
    }

    [Fact]
    public async Task FindByUserAndPasswordNotWorksForLockoutAsExpected()
    {
        // Given
        string inputPassword = RandomString();

        IQueryable<SSOUser> ssoUsersInService = RandomSSOUsers()
            .AsQueryable();

        ssoUserServiceMock.Setup(expression: ssoUserServiceMock =>
            ssoUserServiceMock.GetAllSSOUsers(ignoreFilters: true))
            .Returns(value: ssoUsersInService);

        SSOUser expectedSSOUser = ssoUsersInService.First();

        expectedSSOUser.LockoutEnabled = true;

        passwordHashingBrokerMock.Setup(expression: passwordHashingBrokerMock =>
            passwordHashingBrokerMock.VerifyHashedPassword(
                hashedPassword: expectedSSOUser.PasswordHash,
                providedPassword: inputPassword))
            .Returns(value: PasswordVerificationOutcome.Success);

        // When
        // Then

        SecurityProcessingServiceException actualException =
            await Assert.ThrowsAsync<SecurityProcessingServiceException>(
                testCode: async () =>
                    await ssoUserProcessingService.FindByUserAndPasswordAsync(
                        username: expectedSSOUser.Id,
                        password: inputPassword));


        actualException.InnerException.Should()
            .BeOfType<System.Security.SecurityException>();

    }
}
#pragma warning restore STXFORMAT008