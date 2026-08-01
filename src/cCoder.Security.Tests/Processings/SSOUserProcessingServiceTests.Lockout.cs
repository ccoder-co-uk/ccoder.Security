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

public partial class SSOUserProcessingServiceTests
{
    [Fact]
    public async Task ShouldLockAccountAtConfiguredFailureBoundaryAsync()
    {
        // Given
        DateTimeOffset currentTime = new(
            year: 2026,
            month: 8,
            day: 1,
            hour: 12,
            minute: 0,
            second: 0,
            offset: TimeSpan.Zero);
        string password = RandomString();
        SSOUser user = RandomSSOUser();
        user.AccessFailedCount = securityConfiguration.MaxFailedAccessAttempts - 1;
        user.LockoutEnabled = false;
        IQueryable<SSOUser> users = new[] { user }.AsQueryable();

        ssoUserServiceMock
            .Setup(expression: service => service.GetAllSSOUsers(ignoreFilters: true))
            .Returns(value: users);
        ssoUserServiceMock
            .Setup(expression: service => service.UpdateSSOUserAsync(item: user))
            .ReturnsAsync(value: user);
        passwordHashingBrokerMock
            .Setup(expression: broker => broker.VerifyHashedPassword(
                hashedPassword: user.PasswordHash,
                providedPassword: password))
            .Returns(value: PasswordVerificationOutcome.Failed);
        dateTimeOffsetBrokerMock
            .Setup(expression: broker => broker.GetCurrentTime())
            .Returns(value: currentTime);

        // When
        SecurityProcessingServiceException exception =
            await Assert.ThrowsAsync<SecurityProcessingServiceException>(
                testCode: async () =>
                    await ssoUserProcessingService.FindByUserAndPasswordAsync(
                        username: user.Id,
                        password: password));

        // Then
        exception.InnerException
            .Should()
            .BeOfType<System.Security.SecurityException>();
        user.AccessFailedCount
            .Should()
            .Be(expected: securityConfiguration.MaxFailedAccessAttempts);
        user.LockoutEnabled
            .Should()
            .BeTrue();
        user.LockoutEndDateUtc
            .Should()
            .Be(expected: currentTime.UtcDateTime.AddMinutes(
                value: securityConfiguration.LockoutDurationMinutes));
    }

    [Fact]
    public async Task ShouldAllowAuthenticationAfterTimedLockoutExpiresAsync()
    {
        // Given
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
        string password = RandomString();
        SSOUser user = RandomSSOUser();
        user.AccessFailedCount = securityConfiguration.MaxFailedAccessAttempts;
        user.LockoutEnabled = true;
        user.LockoutEndDateUtc = currentTime.UtcDateTime.AddMinutes(value: -1);
        IQueryable<SSOUser> users = new[] { user }.AsQueryable();

        ssoUserServiceMock
            .Setup(expression: service => service.GetAllSSOUsers(ignoreFilters: true))
            .Returns(value: users);
        ssoUserServiceMock
            .Setup(expression: service => service.UpdateSSOUserAsync(item: user))
            .ReturnsAsync(value: user);
        passwordHashingBrokerMock
            .Setup(expression: broker => broker.VerifyHashedPassword(
                hashedPassword: user.PasswordHash,
                providedPassword: password))
            .Returns(value: PasswordVerificationOutcome.Success);

        dateTimeOffsetBrokerMock
            .Setup(expression: broker => broker.GetCurrentTime())
            .Returns(value: currentTime);

        // When
        SSOUser result = await ssoUserProcessingService
            .FindByUserAndPasswordAsync(
                username: user.Id,
                password: password);

        // Then
        result
            .Should()
            .BeSameAs(expected: user);

        user.AccessFailedCount
            .Should()
            .Be(expected: 0);

        user.LockoutEnabled
            .Should()
            .BeFalse();

        user.LockoutEndDateUtc
            .Should()
            .BeNull();
    }
}