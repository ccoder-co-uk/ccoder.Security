// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Configurations;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Aggregations;

public partial class CurrentUserAggregationServiceTests
{
    [Fact]
    public void InvalidAuthenticationIsRejectedBeforeUserLookup()
    {
        // Given
        Mock<ISSOUserProcessingService> ssoUserProcessingServiceMock =
            new(MockBehavior.Strict);

        ICurrentUserAggregationService currentUserAggregationService =
            new CurrentUserAggregationService(
                ssoUserProcessingService:
                    ssoUserProcessingServiceMock.Object,
                authInfo: new SSOAuthInfo
                {
                    AuthenticationFailed = true
                });

        // When
        Action getCurrentUser = () =>
            currentUserAggregationService.GetCurrentUser();

        // Then
        getCurrentUser.Should()
            .Throw<SecurityAggregationAuthenticationException>();

        ssoUserProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void MeReturnsCurrentUserWithoutProtectedFields()
    {
        // Given
        SSOUser storedUser = new()
        {
            Id = "existing.user",
            DisplayName = "Existing User",
            Email = "existing.user@example.com",
            PhoneNumber = "0123456789",
            PasswordHash = "hashed-password",
            AccessFailedCount = 2,
            EmailConfirmed = true,
            LockoutEnabled = false,
            LockoutEndDateUtc = DateTime.UtcNow,
            PhoneNumberConfirmed = true,
            Roles = new List<SSOUserRole>()
        };

        Mock<ISSOUserProcessingService> ssoUserProcessingServiceMock =
            new(MockBehavior.Strict);

        ICurrentUserAggregationService currentUserAggregationService =
            new CurrentUserAggregationService(
                ssoUserProcessingService:
                    ssoUserProcessingServiceMock.Object,
                authInfo: new SSOAuthInfo());

        ssoUserProcessingServiceMock
            .Setup(expression: service => service.Me())
            .Returns(value: storedUser);

        // When
        SSOUser actualUser = currentUserAggregationService.GetCurrentUser();

        // Then
        actualUser.Should()
            .BeEquivalentTo(expectation: new SSOUser
            {
                Id = storedUser.Id,
                DisplayName = storedUser.DisplayName,
                Email = storedUser.Email,
                PhoneNumber = storedUser.PhoneNumber,
                AccessFailedCount = storedUser.AccessFailedCount,
                EmailConfirmed = storedUser.EmailConfirmed,
                LockoutEnabled = storedUser.LockoutEnabled,
                LockoutEndDateUtc = storedUser.LockoutEndDateUtc,
                PhoneNumberConfirmed = storedUser.PhoneNumberConfirmed
            });

        ssoUserProcessingServiceMock.Verify(expression: service => service.Me(), times: Times.Once);
    }

    [Fact]
    public async Task UpdateMeChangesOnlyEditableProfileFields()
    {
        // Given
        SSOUser storedUser = new()
        {
            Id = "existing.user",
            DisplayName = "Old name",
            Email = "old@example.com",
            PhoneNumber = "0123",
            PasswordHash = "stored-hash",
            AccessFailedCount = 2,
            EmailConfirmed = true,
            LockoutEnabled = false
        };

        SSOUser request = new()
        {
            Id = "attempted.identity.change",
            DisplayName = "New name",
            Email = "new@example.com",
            PhoneNumber = "0456",
            PasswordHash = "attempted-password-change",
            AccessFailedCount = 99,
            LockoutEnabled = true
        };

        Mock<ISSOUserProcessingService> service = new(MockBehavior.Strict);

        service
            .Setup(expression: item => item.Me())
            .Returns(value: storedUser);

        service
            .Setup(expression: item => item.UpdateSSOUserAsync(
                item: It.IsAny<SSOUser>()))
            .Returns(value: new ValueTask<SSOUser>(result: storedUser));

        ICurrentUserAggregationService manager =
            new CurrentUserAggregationService(
                ssoUserProcessingService: service.Object,
                authInfo: new SSOAuthInfo());

        // When
        SSOUser result = await manager.UpdateCurrentSSOUserAsync(
            updatedUser: request);

        // Then
        storedUser.Id
            .Should()
            .Be(expected: "existing.user");

        storedUser.PasswordHash
            .Should()
            .Be(expected: "stored-hash");

        storedUser.AccessFailedCount
            .Should()
            .Be(expected: 2);

        storedUser.LockoutEnabled
            .Should()
            .BeFalse();

        result.DisplayName
            .Should()
            .Be(expected: "New name");

        result.Email
            .Should()
            .Be(expected: "new@example.com");

        result.PhoneNumber
            .Should()
            .Be(expected: "0456");

        result.PasswordHash
            .Should()
            .BeNull();
    }
}