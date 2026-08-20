// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;
using cCoder.Security.Exposures.Controllers;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security;
using System.Security.Claims;
using Xunit;

namespace cCoder.Security.Tests;

public sealed partial class ControllerHttpComplianceTests
{
    [Fact]
    public async Task PostChangePassword_WhenConfirmationDiffers_ShouldReturnBadRequest()
    {
        // Given
        (AuthenticationController controller, Mock<IAuthenticationManager> manager) =
            CreateAuthenticatedPasswordController();

        ChangePasswordRequest request = CreatePasswordRequest();
        request.ConfirmPassword = "different-password";

        // When
        IActionResult result = await controller.PostChangePassword(
            newChangePasswordRequest: request);

        // Then
        result
            .Should()
            .BeOfType<BadRequestObjectResult>();

        manager.Verify(
            expression: service => service.ChangePasswordAsync(
                username: It.IsAny<string>(),
                oldPassword: It.IsAny<string>(),
                newPassword: It.IsAny<string>()),
            times: Times.Never);
    }

    [Theory]
    [InlineData("authentication", StatusCodes.Status401Unauthorized)]
    [InlineData("validation", StatusCodes.Status400BadRequest)]
    [InlineData("security", StatusCodes.Status401Unauthorized)]
    [InlineData("dependency", StatusCodes.Status503ServiceUnavailable)]
    [InlineData("unexpected", StatusCodes.Status500InternalServerError)]
    public async Task PostChangePassword_WhenServiceFails_ShouldReturnSafeStatus(
        string failure,
        int expectedStatus)
    {
        // Given
        (AuthenticationController controller, Mock<IAuthenticationManager> manager) =
            CreateAuthenticatedPasswordController();

        Exception exception = failure switch
        {
            "authentication" => new SecurityAggregationAuthenticationException(
                innerException: new SecurityException()),
            "validation" => new SecurityAggregationValidationException(
                innerException: new ArgumentException()),
            "security" => new SecurityAggregationServiceException(
                innerException: new Exception(
                    message: "wrapped",
                    innerException: new SecurityException())),
            "dependency" => new SecurityAggregationDependencyException(
                innerException: new InvalidOperationException()),
            _ => new InvalidOperationException()
        };

        manager
            .Setup(expression: service => service.ChangePasswordAsync(
                username: "current.user",
                oldPassword: "old-password",
                newPassword: "new-password"))
            .Returns(value: ValueTask.FromException(exception: exception));

        // When
        IActionResult result = await controller.PostChangePassword(
            newChangePasswordRequest: CreatePasswordRequest());

        // Then
        int actualStatus = result switch
        {
            ChallengeResult => StatusCodes.Status401Unauthorized,
            ObjectResult objectResult => objectResult.StatusCode!.Value,
            _ => throw new InvalidOperationException(
                message: $"Unexpected result type {result.GetType().Name}.")
        };

        actualStatus.Should()
            .Be(expected: expectedStatus);
    }

    private static (
        AuthenticationController Controller,
        Mock<IAuthenticationManager> Manager)
        CreateAuthenticatedPasswordController()
    {
        Mock<IAuthenticationManager> authenticationManager = new();
        Mock<ISecurityCurrentUserManager> currentUserManager = new();

        currentUserManager
            .Setup(expression: manager => manager.GetCurrentUser())
            .Returns(value: new SSOUser { Id = "current.user" });

        AuthenticationController controller = new(
            authenticationAggregationService: authenticationManager.Object,
            currentUserManager: currentUserManager.Object);

        controller.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                identity: new ClaimsIdentity(
                    claims: [],
                    authenticationType: "Test"))
        };

        return (controller, authenticationManager);
    }

    private static ChangePasswordRequest CreatePasswordRequest() =>
        new()
        {
            ConfirmPassword = "new-password",
            NewPassword = "new-password",
            OldPassword = "old-password"
        };
}