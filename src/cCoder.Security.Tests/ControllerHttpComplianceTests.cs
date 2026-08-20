// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Exposures.Controllers;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using FluentAssertions;
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
                exception: new SecurityAggregationValidationException(
                    innerException: new Exception(message: "sensitive"))));

        AuthenticationController controller =
            new(
                authenticationAggregationService: authenticationManager.Object,
                currentUserManager: Mock.Of<ISecurityCurrentUserManager>());

        // When
        IActionResult result = await controller.PostLogin(
            newAuth: new Auth { User = "user", Pass = "wrong" });

        // Then
        ObjectResult response = result
            .Should()
            .BeOfType<ObjectResult>()
            .Subject;

        response.StatusCode
            .Should()
            .Be(expected: StatusCodes.Status401Unauthorized);

        response.Value.ToString()
            .Should()
            .NotContain(unexpected: "sensitive");
    }

    [Fact]
    public async Task PostSetup_WhenValidationFails_ShouldReturnSafeBadRequest()
    {
        // Given
        Mock<ITenantManager> tenantManager = new();

        tenantManager
            .Setup(expression: manager => manager.SetupAsync(
                setupDetails: It.IsAny<SetupDetails>()))
            .Returns(value: ValueTask.FromException(
                exception: new SecurityAggregationValidationException(
                    innerException: new Exception(message: "sensitive"))));

        SetupController controller = new(tenantManager: tenantManager.Object);

        // When
        IActionResult result = await controller.PostSetup(
            newSetupDetails: new SetupDetails());

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
            new(tenantAggregationService: tenantManager.Object);

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
    public async Task PostChangePasswordUsesAuthenticatedCurrentUser()
    {
        // Given
        Mock<IAuthenticationManager> authenticationManager = new();
        Mock<ISecurityCurrentUserManager> currentUserManager = new();

        currentUserManager
            .Setup(expression: manager => manager.GetCurrentUser())
            .Returns(value: new SSOUser { Id = "current.user" });

        AuthenticationController controller = new(
            authenticationAggregationService: authenticationManager.Object,
            currentUserManager: currentUserManager.Object);

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
                manager.ChangePasswordAsync(
                    username: "current.user",
                    oldPassword: "old-password",
                    newPassword: "new-password"),
                times: Times.Once);
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

        Mock<ISecurityCurrentUserManager> currentUserManager = new();

        currentUserManager
            .Setup(expression: manager => manager.UpdateCurrentSSOUserAsync(
                updatedUser: request))
            .ReturnsAsync(value: request);

        CurrentUserController controller = new(
            currentUserAggregationService: currentUserManager.Object);

        // When
        IActionResult result = await controller.PutMe(updatedUser: request);

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>();

        currentUserManager
            .Verify(expression: manager =>
                manager.UpdateCurrentSSOUserAsync(updatedUser: request),
                times: Times.Once);
    }
}