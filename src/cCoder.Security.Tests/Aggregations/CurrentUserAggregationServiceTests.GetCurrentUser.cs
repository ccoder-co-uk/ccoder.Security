// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
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
                    ssoUserProcessingServiceMock.Object);

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
}