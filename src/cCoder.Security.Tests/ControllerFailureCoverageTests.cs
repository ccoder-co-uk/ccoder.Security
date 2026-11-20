// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

using cCoder.Security.Brokers.Logging;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures.Controllers;
using cCoder.Security.Models.DTOs;
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
    public async Task ChangePassword_WhenAnonymous_ShouldWarnExactlyOnce()
    {
        // Given
        TestLoggingBroker loggingBroker = new();

        AuthenticationController controller =
            (AuthenticationController)CreateController(
                controllerType: typeof(AuthenticationController),
                exception: null,
                loggingBroker: loggingBroker);

        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(
            identity: new ClaimsIdentity());

        // When
        IActionResult result = await controller.PostChangePassword(
            newChangePasswordRequest: CreatePasswordRequest());

        // Then
        StatusCode(result: result)
            .Should()
            .Be(expected: StatusCodes.Status401Unauthorized);

        AssertLogCounts(
            loggingBroker: loggingBroker,
            information: 0,
            warning: 1,
            error: 0);
    }

    [Fact]
    public async Task ChangePassword_WhenConfirmationDiffers_ShouldWarnExactlyOnce()
    {
        // Given
        TestLoggingBroker loggingBroker = new();

        AuthenticationController controller =
            (AuthenticationController)CreateController(
                controllerType: typeof(AuthenticationController),
                exception: null,
                loggingBroker: loggingBroker);

        ChangePasswordRequest request = CreatePasswordRequest();
        request.ConfirmPassword = "different";

        // When
        IActionResult result = await controller.PostChangePassword(
            newChangePasswordRequest: request);

        // Then
        StatusCode(result: result)
            .Should()
            .Be(expected: StatusCodes.Status400BadRequest);

        AssertLogCounts(
            loggingBroker: loggingBroker,
            information: 0,
            warning: 1,
            error: 0);
    }

    [Fact]
    public async Task EveryNativeInvalidModelBranch_ShouldWarnExactlyOnce()
    {
        // Given
        MethodInfo[] actions = ControllerTypes()
            .SelectMany(selector: controllerType => ActionMethods(
                controllerType: controllerType))
            .Where(predicate: action => action
                .GetParameters()
                .Any(predicate: parameter =>
                    parameter.GetCustomAttribute<FromBodyAttribute>() is not null)
                || (action.Name == "Delete"
                    && (action.DeclaringType == typeof(SSORoleController)
                        || action.DeclaringType == typeof(SSOUserRoleController)
                        || action.DeclaringType == typeof(TenantController))))
            .ToArray();

        // When
        foreach (MethodInfo action in actions)
        {
            TestLoggingBroker loggingBroker = new();

            Controller controller = CreateController(
                controllerType: action.DeclaringType,
                exception: null,
                loggingBroker: loggingBroker);

            controller.ModelState.AddModelError(
                key: "request",
                errorMessage: "invalid");

            IActionResult result = await ResolveResultAsync(
                invocationResult: action.Invoke(
                    obj: controller,
                    parameters: CreateArguments(action: action)));

            StatusCode(result: result)
                .Should()
                .Be(expected: StatusCodes.Status400BadRequest);

            AssertLogCounts(
                loggingBroker: loggingBroker,
                information: 0,
                warning: 1,
                error: 0);
        }

        // Then
        actions.Should()
            .HaveCount(expected: 18);
    }

    [Fact]
    public async Task EveryNativeMissingResourceBranch_ShouldWarnExactlyOnce()
    {
        // Given
        MethodInfo[] actions = ControllerTypes()
            .SelectMany(selector: controllerType => ActionMethods(
                controllerType: controllerType))
            .Where(predicate: action => IsMissingResourceAction(
                action: action))
            .ToArray();

        // When
        foreach (MethodInfo action in actions)
        {
            TestLoggingBroker loggingBroker = new();

            Controller controller = CreateController(
                controllerType: action.DeclaringType,
                exception: null,
                loggingBroker: loggingBroker,
                emptyCollections: true);

            IActionResult result = await ResolveResultAsync(
                invocationResult: action.Invoke(
                    obj: controller,
                    parameters: CreateArguments(action: action)));

            StatusCode(result: result)
                .Should()
                .Be(
                    expected: StatusCodes.Status404NotFound,
                    because: $"{action.DeclaringType.Name}.{action.Name}");

            AssertLogCounts(
                loggingBroker: loggingBroker,
                information: 0,
                warning: 1,
                error: 0);
        }

        // Then
        actions.Should()
            .HaveCount(expected: 10);
    }

    [Fact]
    public async Task EveryControllerAction_WhenSuccessful_ShouldLogInformationExactlyOnce()
    {
        // Given
        IEnumerable<Type> controllerTypes = ControllerTypes();

        // When
        foreach (Type controllerType in controllerTypes)
        {
            foreach (MethodInfo action in ActionMethods(controllerType: controllerType))
            {
                TestLoggingBroker loggingBroker = new();

                Controller controller = CreateController(
                    controllerType: controllerType,
                    exception: null,
                    loggingBroker: loggingBroker);

                object invocationResult = action.Invoke(
                    obj: controller,
                    parameters: CreateArguments(action: action));

                IActionResult result = await ResolveResultAsync(
                    invocationResult: invocationResult);

                StatusCode(result: result)
                    .Should()
                    .BeInRange(
                        minimumValue: StatusCodes.Status200OK,
                        maximumValue: StatusCodes.Status300MultipleChoices - 1,
                        because: $"{controllerType.Name}.{action.Name}");

                loggingBroker.InformationCount
                    .Should()
                    .Be(expected: 1);

                loggingBroker.WarningCount
                    .Should()
                    .Be(expected: 0);

                loggingBroker.ErrorCount
                    .Should()
                    .Be(expected: 0);
            }
        }

        // Then
        controllerTypes
            .Should()
            .NotBeEmpty();
    }

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

            exceptionCase.AssertSafeBody(result: result);
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
        yield return ArgumentCase();
        yield return AuthenticationExceptionCase();
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

    private static ExceptionCase ArgumentCase() =>
        new(
            Exception: new ArgumentException(message: "sensitive-argument-detail"),
            MinimumStatus: StatusCodes.Status400BadRequest,
            MaximumStatus: StatusCodes.Status400BadRequest,
            Warning: true);

    private static ExceptionCase AuthenticationExceptionCase() =>
        new(
            Exception: new AuthenticationException(
                message: "sensitive-authentication-detail"),
            MinimumStatus: StatusCodes.Status401Unauthorized,
            MaximumStatus: StatusCodes.Status401Unauthorized,
            Warning: true);

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
            Exception: new InvalidOperationException(
                message: "sensitive-unexpected-detail"),
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
        TestLoggingBroker loggingBroker,
        bool emptyCollections = false)
    {
        ConstructorInfo constructor = controllerType
            .GetConstructors()
            .Single();

        object[] constructorArguments = constructor
            .GetParameters()
            .Select(selector: parameter =>
                parameter.ParameterType == typeof(ILoggingBroker)
                    ? loggingBroker
                    : exception is null
                        ? SuccessfulDispatchProxy.Create(
                            interfaceType: parameter.ParameterType,
                            emptyCollections: emptyCollections)
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
            return Guid.Empty;
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

    private static ChangePasswordRequest CreatePasswordRequest() =>
        new()
        {
            ConfirmPassword = "new-password",
            NewPassword = "new-password",
            OldPassword = "old-password"
        };

    private static bool IsMissingResourceAction(MethodInfo action)
    {
        Type controllerType = action.DeclaringType;

        if (action.Name == "Delete")
        {
            return controllerType == typeof(SSORoleController)
                || controllerType == typeof(SSOUserController)
                || controllerType == typeof(SSOUserRoleController)
                || controllerType == typeof(TenantController);
        }

        return action.Name == "Get"
            && action.GetParameters().Length == 1
            && action.GetParameters()[0].GetCustomAttribute<FromRouteAttribute>() is not null
            && (controllerType == typeof(SSOPrivilegeController)
                || controllerType == typeof(SSORoleController)
                || controllerType == typeof(SSOUserController)
                || controllerType == typeof(TenantAnalysisController)
                || controllerType == typeof(TenantController)
                || controllerType == typeof(UserEventController));
    }

    private static void AssertLogCounts(
        TestLoggingBroker loggingBroker,
        int information,
        int warning,
        int error)
    {
        loggingBroker.InformationCount.Should()
            .Be(expected: information);

        loggingBroker.WarningCount.Should()
            .Be(expected: warning);

        loggingBroker.ErrorCount.Should()
            .Be(expected: error);
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
                    .Be(expected: 1);

                loggingBroker.ErrorCount
                    .Should()
                    .Be(expected: 0);
            }
            else
            {
                loggingBroker.ErrorCount
                    .Should()
                    .Be(expected: 1);

                loggingBroker.WarningCount
                    .Should()
                    .Be(expected: 0);
            }

            loggingBroker.InformationCount
                .Should()
                .Be(expected: 0);
        }

        internal void AssertSafeBody(IActionResult result)
        {
            string expectedBody = Exception switch
            {
                ArgumentException => "The request is invalid.",
                System.Security.Authentication.AuthenticationException =>
                    "Authentication failed.",
                InvalidOperationException => "The security operation failed.",
                _ => null
            };

            if (expectedBody is null)
            {
                return;
            }

            ObjectResult response = result.Should()
                .BeAssignableTo<ObjectResult>()
                .Subject;

            response.Value.Should()
                .Be(expected: expectedBody);

            response.Value
                .ToString()
                .Should()
                .NotContain(unexpected: Exception.Message);
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

    private class SuccessfulDispatchProxy : DispatchProxy
    {
        internal bool EmptyCollections { get; set; }

        internal static object Create(Type interfaceType, bool emptyCollections)
        {
            SuccessfulDispatchProxy proxy =
                (SuccessfulDispatchProxy)DispatchProxy.Create(
                interfaceType: interfaceType,
                proxyType: typeof(SuccessfulDispatchProxy));

            proxy.EmptyCollections = emptyCollections;

            return proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args) =>
            CreateValue(type: targetMethod.ReturnType);

        private object CreateValue(Type type)
        {
            if (type == typeof(void))
            {
                return null;
            }

            if (type == typeof(string))
            {
                return "value";
            }

            if (type == typeof(Task))
            {
                return Task.CompletedTask;
            }

            if (type == typeof(ValueTask))
            {
                return ValueTask.CompletedTask;
            }

            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                Type valueType = type.GetGenericArguments()[0];

                if (genericType == typeof(IQueryable<>))
                {
                    Array values = Array.CreateInstance(
                        elementType: valueType,
                        length: EmptyCollections ? 0 : 1);

                    if (!EmptyCollections)
                    {
                        values.SetValue(
                            value: CreateEntity(type: valueType),
                            index: 0);
                    }

                    return typeof(Queryable)
                        .GetMethods()
                        .Single(predicate: method =>
                            method.Name == nameof(Queryable.AsQueryable)
                            && method.IsGenericMethod
                            && method.GetParameters().Length == 1)
                        .MakeGenericMethod(typeArguments: [valueType])
                        .Invoke(obj: null, parameters: [values]);
                }

                object value = CreateValue(type: valueType);

                if (genericType == typeof(Task<>))
                {
                    return typeof(Task)
                        .GetMethod(name: nameof(Task.FromResult))
                        .MakeGenericMethod(typeArguments: [valueType])
                        .Invoke(obj: null, parameters: [value]);
                }

                if (genericType == typeof(ValueTask<>))
                {
                    return Activator.CreateInstance(type: type, args: [value]);
                }
            }

            return CreateEntity(type: type);
        }

        private static object CreateEntity(Type type)
        {
            object value = type.IsValueType
                ? Activator.CreateInstance(type: type)
                : type.GetConstructor(types: Type.EmptyTypes)?.Invoke(parameters: null);

            if (value is null)
            {
                return null;
            }

            PropertyInfo idProperty = value
                .GetType()
                .GetProperty(name: "Id");

            SetProperty(
                value: value,
                name: "Id",
                propertyValue: idProperty?.PropertyType == typeof(string)
                    ? "value"
                    : Guid.Empty);

            SetProperty(value: value, name: "UserId", propertyValue: "value");
            SetProperty(value: value, name: "RoleId", propertyValue: Guid.Empty);

            return value;
        }

        private static void SetProperty(object value, string name, object propertyValue)
        {
            PropertyInfo property = value
                .GetType()
                .GetProperty(name: name);

            if (property?.CanWrite == true
                && propertyValue is not null
                && property.PropertyType.IsInstanceOfType(o: propertyValue))
            {
                property.SetValue(obj: value, value: propertyValue);
            }
        }
    }

    private sealed class TestLoggingBroker : ILoggingBroker
    {
        internal int WarningCount { get; private set; }

        internal int ErrorCount { get; private set; }

        internal int InformationCount { get; private set; }

        public void LogInformation(string message, params object[] args) =>
            InformationCount++;

        public void LogWarning(string message, params object[] args) =>
            WarningCount++;

        public void LogError(
            Exception exception,
            string message,
            params object[] args) =>
            ErrorCount++;
    }
}