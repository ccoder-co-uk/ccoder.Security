// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Security.Data.Models;
using cCoder.Security.Brokers.Logging;
using cCoder.Security.Exposures;
using cCoder.Security.Exposures.Controllers;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using FluentAssertions;
using System.Security;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Security.Tests;

public sealed partial class ControllerHttpComplianceTests
{
    [Fact]
    public async Task PostLogin_WhenCredentialsAreInvalid_ShouldReturnUnauthorized()
    {
        // Given
        Mock<IAuthenticationManager> authenticationManager = new();

        authenticationManager
            .Setup(expression: manager => manager.LoginAsync(
                username: It.IsAny<string>(),
                password: It.IsAny<string>()))
            .Returns(value: ValueTask.FromException<Token>(
                exception: new SecurityAggregationAuthenticationException(
                    innerException: new Exception(message: "sensitive"))));

        AuthenticationController controller =
            new(
                authenticationAggregationService: authenticationManager.Object,
                loggingBroker: Mock.Of<ILoggingBroker>());

        // When
        IActionResult result = await controller.PostLogin(
            newAuth: new Auth { User = "user", Pass = "wrong" });

        // Then
        result
            .Should()
            .BeOfType<ChallengeResult>();
    }

    [Fact]
    public async Task PostSetup_WhenValidationFails_ShouldReturnSafeBadRequest()
    {
        // Given
        Mock<IRegistrationAggregationService> registrationAggregationService = new();

        registrationAggregationService
            .Setup(expression: service => service.SetupRegisterUserAsync(
                newRegisterUser: It.IsAny<RegisterUser>()))
            .Returns(value: ValueTask.FromException(
                exception: new SecurityAggregationValidationException(
                    innerException: new Exception(message: "sensitive"))));

        SetupController controller = new(
            registrationAggregationService: registrationAggregationService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When
        IActionResult result = await controller.PostSetup(
            newSetupDetails: new SetupDetails
            {
                Tenant = new Tenant(),
                User = new SSOUser()
            });

        // Then
        BadRequestObjectResult response =
            result
                .Should()
                .BeOfType<BadRequestObjectResult>()
                .Subject;

        response.Value.ToString()
            .Should()
            .NotContain(unexpected: "sensitive");
    }

    [Fact]
    public async Task PostTenant_WhenSuccessful_ShouldReturnCreated()
    {
        // Given
        Tenant tenant = new();
        Mock<ITenantAdministrationManager> tenantManager = new();

        tenantManager
            .Setup(expression: manager => manager.AddTenantAsync(item: tenant))
            .ReturnsAsync(value: tenant);

        TenantController controller =
            new(
                tenantAdministrationManager: tenantManager.Object,
                loggingBroker: Mock.Of<ILoggingBroker>());

        // When
        IActionResult result = await controller.Post(newTenant: tenant);

        // Then
        ObjectResult response = result
            .Should()
            .BeOfType<ObjectResult>()
            .Subject;

        response.StatusCode
            .Should()
            .Be(expected: StatusCodes.Status201Created);

        response.Value
            .Should()
            .BeSameAs(expected: tenant);
    }

    [Fact]
    public async Task PostTenant_WhenUnexpectedFailureOccurs_ShouldLogException()
    {
        // Given
        Tenant tenant = new();
        Exception exception = new(message: "tenant persistence failed");
        Mock<ITenantAdministrationManager> tenantManager = new();
        Mock<ILoggingBroker> loggingBroker = new();

        tenantManager
            .Setup(expression: manager => manager.AddTenantAsync(item: tenant))
            .Returns(value: ValueTask.FromException<Tenant>(exception: exception));

        TenantController controller = new(
            tenantAdministrationManager: tenantManager.Object,
            loggingBroker: loggingBroker.Object);

        // When
        IActionResult result = await controller.Post(newTenant: tenant);

        // Then
        ObjectResult response = result
            .Should()
            .BeOfType<ObjectResult>()
            .Subject;

        response.StatusCode
            .Should()
            .Be(expected: StatusCodes.Status500InternalServerError);

        loggingBroker.Verify(
            expression: broker => broker.LogError(
                exception: exception,
                message: "Controller request failed."),
            times: Times.Once);
    }

    [Fact]
    public async Task PostTenant_WhenSecurityFailureOccurs_ShouldWarnAndReturnForbidden()
    {
        // Given
        Tenant tenant = new();
        SecurityException exception = new(message: "access denied");
        Mock<ITenantAdministrationManager> tenantManager = new();
        Mock<ILoggingBroker> loggingBroker = new();

        tenantManager
            .Setup(expression: manager => manager.AddTenantAsync(item: tenant))
            .Returns(value: ValueTask.FromException<Tenant>(exception: exception));

        TenantController controller = new(
            tenantAdministrationManager: tenantManager.Object,
            loggingBroker: loggingBroker.Object);

        // When
        IActionResult result = await controller.Post(newTenant: tenant);

        // Then
        StatusCodeResult response = result
            .Should()
            .BeOfType<StatusCodeResult>()
            .Subject;

        response.StatusCode
            .Should()
            .Be(expected: StatusCodes.Status403Forbidden);

        loggingBroker.Verify(
            expression: broker => broker.LogWarning(
                message: exception.Message,
                args: It.IsAny<object[]>()),
            times: Times.Once);
    }

    [Fact]
    public async Task PostChangePasswordUsesAuthenticationServiceCurrentUser()
    {
        // Given
        Mock<IAuthenticationManager> authenticationManager = new();

        AuthenticationController controller = new(
            authenticationAggregationService: authenticationManager.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        controller.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                identity: new ClaimsIdentity(
                    claims: [],
                    authenticationType: "Test"))
        };

        ChangePasswordRequest request = new()
        {
            OldPassword = "old-password",
            NewPassword = "new-password",
            ConfirmPassword = "new-password"
        };

        // When
        IActionResult result = await controller.PostChangePassword(
            newChangePasswordRequest: request);

        // Then
        result
            .Should()
            .BeOfType<OkResult>();

        authenticationManager
            .Verify(expression: manager =>
                manager.ChangeCurrentUserPasswordAsync(
                    oldPassword: "old-password",
                    newPassword: "new-password"),
                times: Times.Once);
    }

    [Fact]
    public async Task PostChangePassword_WhenAnonymous_ShouldReturnChallenge()
    {
        // Given
        Mock<IAuthenticationManager> authenticationManager = new();

        AuthenticationController controller = new(
            authenticationAggregationService: authenticationManager.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        ChangePasswordRequest request = new()
        {
            OldPassword = "old-password",
            NewPassword = "new-password",
            ConfirmPassword = "new-password"
        };

        // When
        IActionResult result = await controller.PostChangePassword(
            newChangePasswordRequest: request);

        // Then
        result
            .Should()
            .BeOfType<ChallengeResult>();

        authenticationManager.Verify(
            expression: manager => manager.ChangeCurrentUserPasswordAsync(
                oldPassword: It.IsAny<string>(),
                newPassword: It.IsAny<string>()),
            times: Times.Never);
    }

    [Fact]
    public async Task PostForgotPassword_ShouldQueueTokenBackedReset()
    {
        // Given
        Mock<IAuthenticationManager> authenticationManager = new();

        ForgotPasswordController controller = new(
            authenticationAggregationService: authenticationManager.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        ForgotPasswordRequest request = new()
        {
            AppId = 21,
            Email = "user@example.test"
        };

        // When
        IActionResult result = await controller.PostForgotPassword(
            newForgotPasswordRequest: request);

        // Then
        result
            .Should()
            .BeOfType<OkResult>();

        authenticationManager.Verify(
            expression: manager => manager.ForgotPasswordAsync(
                email: "user@example.test"),
            times: Times.Once);
    }

    [Fact]
    public async Task PostConfirmForgotPassword_ShouldConsumeToken()
    {
        // Given
        Mock<IAuthenticationManager> authenticationManager = new();

        ConfirmForgotPasswordController controller = new(
            authenticationAggregationService: authenticationManager.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        ConfirmForgotPasswordRequest request = new()
        {
            ConfirmPassword = "new-password",
            NewPassword = "new-password",
            SourceAppId = 21,
            Token = "reset-token",
            UserId = "current.user"
        };

        // When
        IActionResult result = await controller.PostConfirmForgotPassword(
            newConfirmForgotPasswordRequest: request);

        // Then
        result
            .Should()
            .BeOfType<OkResult>();

        authenticationManager.Verify(
            expression: manager => manager.ConfirmForgotPasswordAsync(
                tokenId: "reset-token",
                userId: "current.user",
                newPassword: "new-password",
                confirmNewPassword: "new-password"),
            times: Times.Once);
    }

    [Fact]
    public async Task PostConfirmForgotPassword_WhenTokenIsInvalid_ShouldReturnBadRequest()
    {
        // Given
        Mock<IAuthenticationManager> authenticationManager = new();

        authenticationManager
            .Setup(expression: manager => manager.ConfirmForgotPasswordAsync(
                tokenId: "invalid-token",
                userId: "current.user",
                newPassword: "new-password",
                confirmNewPassword: "new-password"))
            .Returns(value: ValueTask.FromException(
                exception: new SecurityAggregationValidationException(
                    innerException: new SecurityException())));

        ConfirmForgotPasswordController controller = new(
            authenticationAggregationService: authenticationManager.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        ConfirmForgotPasswordRequest request = new()
        {
            ConfirmPassword = "new-password",
            NewPassword = "new-password",
            SourceAppId = 21,
            Token = "invalid-token",
            UserId = "current.user"
        };

        // When
        IActionResult result = await controller.PostConfirmForgotPassword(
            newConfirmForgotPasswordRequest: request);

        // Then
        result
            .Should()
            .BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task PutMeUsesSelfServiceManager()
    {
        // Given
        SSOUser request = new()
        {
            DisplayName = "New name",
            Email = "new@example.com"
        };

        Mock<ISSOUserAggregationService> ssoUserAggregationService = new();

        ssoUserAggregationService
            .Setup(expression: service => service.UpdateCurrentSSOUserAsync(
                updatedSSOUser: request))
            .ReturnsAsync(value: request);

        CurrentUserController controller = new(
            ssoUserAggregationService: ssoUserAggregationService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When
        IActionResult result = await controller.PutMe(updatedSSOUser: request);

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>();

        ssoUserAggregationService
            .Verify(expression: service =>
                service.UpdateCurrentSSOUserAsync(updatedSSOUser: request),
                times: Times.Once);
    }
}
