// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

using cCoder.Security.Brokers.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Security.Web.Services.Foundations;
using Xunit;
using CurrentUserController = Security.Web.Controllers.CurrentUserController;
using HomeController = Security.Web.Controllers.HomeController;

namespace cCoder.Security.Tests.Exposures;

public sealed partial class WebControllerResponseContractTests
{
    [Fact]
    public void CurrentUserGet_WhenSuccessful_LogsInformationExactlyOnce()
    {
        // Given
        Mock<ICurrentUserService> service = new();
        Mock<ILoggingBroker> loggingBroker = new();

        service
            .Setup(expression: instance => instance.GetCurrentUserId())
            .Returns(value: "user");

        CurrentUserController controller = new(
            currentUserService: service.Object,
            loggingBroker: loggingBroker.Object);

        // When
        IActionResult result = controller.Get();

        // Then
        result.Should()
            .BeOfType<OkObjectResult>();

        VerifySuccessLog(loggingBroker: loggingBroker);
    }

    [Fact]
    public void CurrentUserGet_WhenUnexpectedFailureOccurs_ReturnsSafe500AndLogsError()
    {
        // Given
        Exception exception = new(message: "sensitive-current-user-detail");
        Mock<ICurrentUserService> service = new();
        Mock<ILoggingBroker> loggingBroker = new();

        service
            .Setup(expression: instance => instance.GetCurrentUserId())
            .Throws(exception: exception);

        CurrentUserController controller = new(
            currentUserService: service.Object,
            loggingBroker: loggingBroker.Object);

        // When
        IActionResult result = controller.Get();

        // Then
        VerifySafe500(
            result: result,
            sensitiveDetail: exception.Message);

        VerifyErrorLog(
            loggingBroker: loggingBroker,
            exception: exception);
    }

    [Fact]
    public void HomeGet_WhenSuccessful_LogsInformationExactlyOnce()
    {
        // Given
        Mock<IHomeService> service = new();
        Mock<ILoggingBroker> loggingBroker = new();

        service
            .Setup(expression: instance => instance.GetIndexPath())
            .Returns(value: "index.html");

        HomeController controller = new(
            homeService: service.Object,
            loggingBroker: loggingBroker.Object);

        // When
        IActionResult result = controller.Get();

        // Then
        result.Should()
            .BeOfType<PhysicalFileResult>();

        VerifySuccessLog(loggingBroker: loggingBroker);
    }

    [Fact]
    public void HomeGet_WhenUnexpectedFailureOccurs_ReturnsSafe500AndLogsError()
    {
        // Given
        Exception exception = new(message: "sensitive-home-detail");
        Mock<IHomeService> service = new();
        Mock<ILoggingBroker> loggingBroker = new();

        service
            .Setup(expression: instance => instance.GetIndexPath())
            .Throws(exception: exception);

        HomeController controller = new(
            homeService: service.Object,
            loggingBroker: loggingBroker.Object);

        // When
        IActionResult result = controller.Get();

        // Then
        VerifySafe500(
            result: result,
            sensitiveDetail: exception.Message);

        VerifyErrorLog(
            loggingBroker: loggingBroker,
            exception: exception);
    }

    private static void VerifySuccessLog(Mock<ILoggingBroker> loggingBroker)
    {
        loggingBroker.Verify(
            expression: broker => broker.LogInformation(
                message: "Controller request succeeded.",
                args: It.IsAny<object[]>()),
            times: Times.Once);

        loggingBroker.Verify(
            expression: broker => broker.LogWarning(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Never);

        loggingBroker.Verify(
            expression: broker => broker.LogError(
                exception: It.IsAny<Exception>(),
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Never);
    }

    private static void VerifyErrorLog(
        Mock<ILoggingBroker> loggingBroker,
        Exception exception)
    {
        loggingBroker.Verify(
            expression: broker => broker.LogInformation(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Never);

        loggingBroker.Verify(
            expression: broker => broker.LogWarning(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Never);

        loggingBroker.Verify(
            expression: broker => broker.LogError(
                exception: exception,
                message: "Controller request failed.",
                args: It.IsAny<object[]>()),
            times: Times.Once);
    }

    private static void VerifySafe500(
        IActionResult result,
        string sensitiveDetail)
    {
        ObjectResult response = result.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        response.StatusCode.Should()
            .Be(expected: StatusCodes.Status500InternalServerError);

        response.Value
            .ToString()
            .Should()
            .NotContain(unexpected: sensitiveDetail);
    }
}