// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;

using cCoder.Security.Brokers.Logging;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures.Controllers;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Xunit;

namespace cCoder.Security.Tests;

public sealed partial class ControllerFailureCoverageTests
{
    [Fact]
    public async Task EveryControllerAction_ShouldClassifyAndLogFailures()
    {
        // Given
        IEnumerable<Type> controllerTypes = ControllerTypes();

        // When
        foreach (Type controllerType in controllerTypes)
        {
            foreach (ExceptionCase exceptionCase in ExceptionCases(
                controllerType: controllerType))
            {
                await VerifyEveryActionAsync(
                    controllerType: controllerType,
                    exceptionCase: exceptionCase);
            }
        }

        // Then
        controllerTypes
            .Should()
            .NotBeEmpty();
    }

    private static async Task VerifyEveryActionAsync(
        Type controllerType,
        ExceptionCase exceptionCase)
    {
        foreach (MethodInfo action in ActionMethods(controllerType: controllerType))
        {
            TestLoggingBroker loggingBroker = new();

            Controller controller = CreateController(
                controllerType: controllerType,
                exception: exceptionCase.Exception,
                loggingBroker: loggingBroker);

            object invocationResult = action.Invoke(
                obj: controller,
                parameters: CreateArguments(action: action));

            IActionResult result = await ResolveResultAsync(
                invocationResult: invocationResult);

            int statusCode = StatusCode(result: result);

            exceptionCase.AssertStatus(
                statusCode: statusCode,
                scenario: $"{controllerType.Name}.{action.Name} with "
                    + exceptionCase.Exception.GetType().Name);

            exceptionCase.AssertLog(loggingBroker: loggingBroker);
        }
    }

    private static IEnumerable<Type> ControllerTypes()
    {
        return
        [
            typeof(AuthenticationController),
            typeof(ConfirmForgotPasswordController),
            typeof(CurrentUserController),
            typeof(ForgotPasswordController),
            typeof(RegistrationController),
            typeof(SetupController),
            typeof(SSOPrivilegeController),
            typeof(SSORoleController),
            typeof(SSOUserController),
            typeof(SSOUserRoleController),
            typeof(TenantAnalysisController),
            typeof(TenantController),
            typeof(UserEventController)
        ];
    }

    private static IEnumerable<MethodInfo> ActionMethods(Type controllerType) =>
        controllerType
            .GetMethods(bindingAttr:
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.DeclaredOnly)
            .Where(predicate: method => method
                .GetCustomAttributes<HttpMethodAttribute>(inherit: true)
                .Any());

    private static IEnumerable<ExceptionCase> ExceptionCases(Type controllerType)
    {
        yield return ValidationCase(controllerType: controllerType);
        yield return DependencyCase(controllerType: controllerType);
        yield return SecurityCase();
        yield return UnexpectedCase();

        if (controllerType == typeof(AuthenticationController)
            || controllerType == typeof(CurrentUserController))
        {
            yield return AuthenticationCase();
        }
    }

    private static ExceptionCase ValidationCase(Type controllerType) =>
        IsProcessingController(controllerType: controllerType)
            ? new(
                Exception: new SecurityProcessingValidationException(
                    innerException: new ArgumentException()),
                MinimumStatus: StatusCodes.Status400BadRequest,
                MaximumStatus: StatusCodes.Status499ClientClosedRequest,
                Warning: true)
            : IsOrchestrationController(controllerType: controllerType)
                ? new(
                    Exception: new SecurityOrchestrationValidationException(
                        innerException: new ArgumentException()),
                    MinimumStatus: StatusCodes.Status400BadRequest,
                    MaximumStatus: StatusCodes.Status499ClientClosedRequest,
                    Warning: true)
                : new(
                    Exception: new SecurityAggregationValidationException(
                        innerException: new ArgumentException()),
                    MinimumStatus: StatusCodes.Status400BadRequest,
                    MaximumStatus: StatusCodes.Status499ClientClosedRequest,
                    Warning: true);

    private static ExceptionCase DependencyCase(Type controllerType) =>
        IsProcessingController(controllerType: controllerType)
            ? new(
                Exception: new SecurityProcessingDependencyException(
                    innerException: new InvalidOperationException()),
                MinimumStatus: StatusCodes.Status500InternalServerError,
                MaximumStatus: 599,
                Warning: false)
            : IsOrchestrationController(controllerType: controllerType)
                ? new(
                    Exception: new SecurityOrchestrationDependencyException(
                        innerException: new InvalidOperationException()),
                    MinimumStatus: StatusCodes.Status500InternalServerError,
                    MaximumStatus: 599,
                    Warning: false)
                : new(
                    Exception: new SecurityAggregationDependencyException(
                        innerException: new InvalidOperationException()),
                    MinimumStatus: StatusCodes.Status500InternalServerError,
                    MaximumStatus: 599,
                    Warning: false);

    private static ExceptionCase AuthenticationCase() =>
        new(
            Exception: new SecurityAggregationAuthenticationException(
                innerException: new SecurityException()),
            MinimumStatus: StatusCodes.Status401Unauthorized,
            MaximumStatus: StatusCodes.Status401Unauthorized,
            Warning: true);

    private static ExceptionCase SecurityCase() =>
        new(
            Exception: new SecurityException(message: "access denied"),
            MinimumStatus: StatusCodes.Status403Forbidden,
            MaximumStatus: StatusCodes.Status403Forbidden,
            Warning: true);

    private static ExceptionCase UnexpectedCase() =>
        new(
            Exception: new InvalidOperationException(message: "failed"),
            MinimumStatus: StatusCodes.Status500InternalServerError,
            MaximumStatus: 599,
            Warning: false);

    private static bool IsProcessingController(Type controllerType) =>
        controllerType == typeof(SSOPrivilegeController)
        || controllerType == typeof(TenantAnalysisController)
        || controllerType == typeof(UserEventController);

    private static bool IsOrchestrationController(Type controllerType) =>
        controllerType == typeof(SSORoleController)
        || controllerType == typeof(SSOUserRoleController);

    private static Controller CreateController(
        Type controllerType,
        Exception exception,
        TestLoggingBroker loggingBroker)
    {
        ConstructorInfo constructor = controllerType
            .GetConstructors()
            .Single();

        object[] constructorArguments = constructor
            .GetParameters()
            .Select(selector: parameter =>
                parameter.ParameterType == typeof(ILoggingBroker)
                    ? loggingBroker
                    : ThrowingDispatchProxy.Create(
                        interfaceType: parameter.ParameterType,
                        exception: exception))
            .ToArray();

        Controller controller = (Controller)constructor.Invoke(
            parameters: constructorArguments);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    identity: new ClaimsIdentity(
                        claims: [],
                        authenticationType: "Test"))
            }
        };

        return controller;
    }

    private static object[] CreateArguments(MethodInfo action) =>
        action
            .GetParameters()
            .Select(selector: parameter => CreateArgument(
                parameterType: parameter.ParameterType))
            .ToArray();

    private static object CreateArgument(Type parameterType)
    {
        if (parameterType == typeof(string))
        {
            return "value";
        }

        if (parameterType == typeof(Guid))
        {
            return Guid.NewGuid();
        }

        if (parameterType == typeof(SetupDetails))
        {
            return new SetupDetails
            {
                Tenant = new Tenant(),
                User = new SSOUser()
            };
        }

        if (parameterType.IsValueType)
        {
            return Activator.CreateInstance(type: parameterType);
        }

        if (parameterType.IsInterface || parameterType.IsAbstract)
        {
            return null;
        }

        if (parameterType.GetConstructor(types: Type.EmptyTypes) is null)
        {
            return null;
        }

        return Activator.CreateInstance(type: parameterType);
    }

    private static async Task<IActionResult> ResolveResultAsync(
        object invocationResult)
    {
        if (invocationResult is IActionResult actionResult)
        {
            return actionResult;
        }

        if (invocationResult is Task task)
        {
            await task;

            return (IActionResult)task
                .GetType()
                .GetProperty(name: "Result")
                .GetValue(obj: task);
        }

        MethodInfo asTask = invocationResult
            .GetType()
            .GetMethod(name: "AsTask");

        Task valueTask = (Task)asTask.Invoke(
            obj: invocationResult,
            parameters: null);

        await valueTask;

        return (IActionResult)valueTask
            .GetType()
            .GetProperty(name: "Result")
            .GetValue(obj: valueTask);
    }

    private static int StatusCode(IActionResult result) =>
        result switch
        {
            ChallengeResult => StatusCodes.Status401Unauthorized,
            StatusCodeResult response => response.StatusCode,
            ObjectResult response => response.StatusCode
                ?? StatusCodes.Status200OK,
            _ => StatusCodes.Status200OK
        };

    private sealed record ExceptionCase(
        Exception Exception,
        int MinimumStatus,
        int MaximumStatus,
        bool Warning)
    {
        internal void AssertStatus(int statusCode, string scenario) =>
            statusCode
                .Should()
                .BeInRange(
                    minimumValue: MinimumStatus,
                    maximumValue: MaximumStatus,
                    because: scenario);

        internal void AssertLog(TestLoggingBroker loggingBroker)
        {
            if (Warning)
            {
                loggingBroker.WarningCount
                    .Should()
                    .BeGreaterThan(expected: 0);
            }
            else
            {
                loggingBroker.ErrorCount
                    .Should()
                    .BeGreaterThan(expected: 0);
            }
        }
    }

    private class ThrowingDispatchProxy : DispatchProxy
    {
        internal Exception Exception { get; set; }

        internal static object Create(Type interfaceType, Exception exception)
        {
            ThrowingDispatchProxy proxy = (ThrowingDispatchProxy)DispatchProxy.Create(
                interfaceType: interfaceType,
                proxyType: typeof(ThrowingDispatchProxy));

            proxy.Exception = exception;

            return proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args) =>
            throw Exception;
    }

    private sealed class TestLoggingBroker : ILoggingBroker
    {
        internal int WarningCount { get; private set; }

        internal int ErrorCount { get; private set; }

        public void LogWarning(string message, params object[] args) =>
            WarningCount++;

        public void LogError(
            Exception exception,
            string message,
            params object[] args) =>
            ErrorCount++;
    }
}