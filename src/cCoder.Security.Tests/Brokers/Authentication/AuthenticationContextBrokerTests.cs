// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Authentication;
using cCoder.Security.Models.Configurations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Brokers.Authentication;

public sealed partial class AuthenticationContextBrokerTests
{
    [Fact]
    public void ShouldReturnAuthenticatedUserId()
    {
        // Given
        Mock<ISSOAuthInfo> authInfo = new();

        authInfo
            .SetupGet(expression: info => info.SSOUserId)
            .Returns(value: "user-1");

        AuthenticationContextBroker broker = new(authInfo: authInfo.Object);

        // When
        string actual = broker.GetSSOUserId();

        // Then
        actual.Should()
            .Be(expected: "user-1");
    }

    [Fact]
    public void ShouldReturnGuestWhenAuthenticationContextIsMissing()
    {
        // Given
        AuthenticationContextBroker broker = new(authInfo: null);

        // When
        string actual = broker.GetSSOUserId();

        // Then
        actual.Should()
            .Be(expected: "Guest");
    }

    [Fact]
    public void ShouldReturnGuestWhenUserIdIsMissing()
    {
        // Given
        Mock<ISSOAuthInfo> authInfo = new();
        AuthenticationContextBroker broker = new(authInfo: authInfo.Object);

        // When
        string actual = broker.GetSSOUserId();

        // Then
        actual.Should()
            .Be(expected: "Guest");
    }
}