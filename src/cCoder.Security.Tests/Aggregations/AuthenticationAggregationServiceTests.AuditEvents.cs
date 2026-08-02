// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Events;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Aggregations;

public partial class AuthenticationAggregationServiceTests
{
    [Fact]
    public async Task IssueTokenRaisesOneRedactedSecurityEvent()
    {
        // Given
        string userId = RandomString();
        SSOUser user = new() { Id = userId };
        Token token = new() { Id = RandomString() };

        tokenProcessingServiceMock
            .Setup(expression: service => service.AddTokenForUserIdAsync(
                userId: userId,
                tokenUse: TokenUse.Auth))
            .ReturnsAsync(value: token);

        ssoUserProcessingServiceMock
            .Setup(expression: service => service.FindById(
                ssoUserId: userId))
            .Returns(value: user);

        // When
        Token actualToken = await authenticationAggregationService.IssueTokenAsync(
            userId: userId,
            tokenUse: TokenUse.Auth);

        // Then
        actualToken.Should()
            .BeSameAs(expected: token);

        VerifySingleAccountEvent(
            expectedKind: SecurityAccountEventKind.TokenIssued,
            expectedUser: user);
    }

    [Fact]
    public async Task LoginRaisesOneRedactedSuccessEvent()
    {
        // Given
        string username = RandomString();
        string password = RandomString();
        SSOUser user = new() { Id = username };
        Token token = new() { Id = RandomString() };

        ssoUserProcessingServiceMock
            .Setup(expression: service => service.FindByUserAndPasswordAsync(
                username: username,
                password: password))
            .ReturnsAsync(value: user);

        tokenProcessingServiceMock
            .Setup(expression: service => service.AddTokenForUserIdAsync(
                userId: username,
                tokenUse: TokenUse.Auth))
            .ReturnsAsync(value: token);

        // When
        await authenticationAggregationService.LoginAsync(
            username: username,
            password: password);

        // Then
        VerifySingleAccountEvent(
            expectedKind: SecurityAccountEventKind.LoginSucceeded,
            expectedUser: user);
    }

    [Fact]
    public async Task LogoutRaisesOneRedactedSuccessEvent()
    {
        // Given
        string tokenId = RandomString();
        SSOUser user = new() { Id = RandomString() };

        sessionProcessingServiceMock
            .Setup(expression: service => service.GetUser())
            .Returns(value: user);

        sessionProcessingServiceMock
            .Setup(expression: service => service.GetString(
                key: "token"))
            .Returns(value: tokenId);

        // When
        await authenticationAggregationService.LogoutAsync();

        // Then
        VerifySingleAccountEvent(
            expectedKind: SecurityAccountEventKind.LogoutSucceeded,
            expectedUser: user);
    }

    [Fact]
    public async Task FailedLoginRaisesOneRedactedFailureEvent()
    {
        // Given
        string username = RandomString();
        string password = RandomString();

        // When
        await Assert.ThrowsAnyAsync<Exception>(
            testCode: async () =>
                await authenticationAggregationService.LoginAsync(
                    username: username,
                    password: password));

        // Then
        VerifySingleAccountEvent(
            expectedKind: SecurityAccountEventKind.AuthenticationFailed,
            expectedUser: null);
    }

    [Fact]
    public void OrdinaryNonSecurityActivityRaisesNoAuditEvent()
    {
        // Given
        Times expectedTimes = Times.Never();

        // When
        // Then
        accountEventProcessingServiceMock.Verify(
            expression: service => service.RaiseSecurityAccountEventRequestAsync(
                accountEventRequest: It.IsAny<SecurityAccountEventRequest>()),
            times: expectedTimes);
    }

    private void VerifySingleAccountEvent(
        SecurityAccountEventKind expectedKind,
        SSOUser expectedUser)
    {
        accountEventProcessingServiceMock.Verify(
            expression: service => service.RaiseSecurityAccountEventRequestAsync(
                accountEventRequest: It.Is<SecurityAccountEventRequest>(match: request =>
                    request.Kind == expectedKind
                    && request.User == expectedUser
                    && request.Token == null
                    && request.RegisterForm == null)),
            times: Times.Once());

        accountEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}