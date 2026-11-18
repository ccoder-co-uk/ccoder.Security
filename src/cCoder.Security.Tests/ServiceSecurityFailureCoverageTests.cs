// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;

using cCoder.Security.Exposures.Controllers;
using FluentAssertions;
using Xunit;

namespace cCoder.Security.Tests;

public sealed partial class ServiceSecurityFailureCoverageTests
{
    [Fact]
    public async Task EveryReachableServiceMethod_ShouldClassifySecurityFailures()
    {
        // Given
        IEnumerable<Type> serviceTypes = ServiceTypes();
        int classifiedSecurityFailures = 0;

        // When
        foreach (Type serviceType in serviceTypes)
        {
            object service = TryCreateService(serviceType: serviceType);

            if (service is null)
            {
                continue;
            }

            foreach (MethodInfo method in ServiceMethods(serviceType: serviceType))
            {
                Exception exception = await InvokeAsync(
                    service: service,
                    method: method);

                if (ContainsSecurityException(exception: exception))
                {
                    exception
                        .Should()
                        .NotBeOfType<SecurityException>();

                    classifiedSecurityFailures++;
                }
            }
        }

        // Then
        classifiedSecurityFailures
            .Should()
            .BeGreaterThan(expected: 50);
    }

    private static IEnumerable<Type> ServiceTypes() =>
        typeof(AuthenticationController)
            .Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsClass
                && !type.IsAbstract
                && type.Namespace is not null
                && type.Namespace.StartsWith(
                    value: "cCoder.Security.Services.",
                    comparisonType: StringComparison.Ordinal)
                && type.Name.EndsWith(
                    value: "Service",
                    comparisonType: StringComparison.Ordinal));

    private static IEnumerable<MethodInfo> ServiceMethods(Type serviceType) =>
        serviceType
            .GetMethods(bindingAttr:
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.DeclaredOnly)
            .Where(predicate: method =>
                !method.IsSpecialName
                && !method.ContainsGenericParameters);

    private static object TryCreateService(Type serviceType)
    {
        ConstructorInfo constructor = serviceType
            .GetConstructors(bindingAttr:
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic)
            .SingleOrDefault();

        if (constructor is null)
        {
            return null;
        }

        object[] arguments = constructor
            .GetParameters()
            .Select(selector: parameter => CreateDependency(
                dependencyType: parameter.ParameterType))
            .ToArray();

        if (arguments.Any(predicate: argument => argument is UnavailableValue))
        {
            return null;
        }

        try
        {
            return constructor.Invoke(parameters: arguments);
        }
        catch (TargetInvocationException)
        {
            return null;
        }
    }

    private static object CreateDependency(Type dependencyType)
    {
        if (dependencyType.IsInterface)
        {
            return ThrowingDispatchProxy.Create(
                interfaceType: dependencyType,
                exception: new SecurityException(message: "access denied"));
        }

        if (dependencyType.IsValueType)
        {
            return Activator.CreateInstance(type: dependencyType);
        }

        if (dependencyType.GetConstructor(types: Type.EmptyTypes) is not null)
        {
            return Activator.CreateInstance(type: dependencyType);
        }

        return UnavailableValue.Instance;
    }

    private static async Task<Exception> InvokeAsync(
        object service,
        MethodInfo method)
    {
        try
        {
            object invocationResult = method.Invoke(
                obj: service,
                parameters: method
                    .GetParameters()
                    .Select(selector: parameter => CreateArgument(
                        parameterType: parameter.ParameterType))
                    .ToArray());

            await AwaitAsync(invocationResult: invocationResult);

            return null;
        }
        catch (TargetInvocationException exception)
        {
            return exception.InnerException;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    private static async Task AwaitAsync(object invocationResult)
    {
        if (invocationResult is null)
        {
            return;
        }

        if (invocationResult is Task task)
        {
            await task;
            return;
        }

        MethodInfo asTask = invocationResult
            .GetType()
            .GetMethod(name: "AsTask");

        if (asTask is not null)
        {
            await (Task)asTask.Invoke(
                obj: invocationResult,
                parameters: null);
        }
    }

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

        if (parameterType.IsArray)
        {
            return Array.CreateInstance(
                elementType: parameterType.GetElementType(),
                length: 0);
        }

        if (parameterType.IsEnum)
        {
            return Enum.GetValues(enumType: parameterType)
                .GetValue(index: 0);
        }

        if (parameterType.IsValueType)
        {
            return Activator.CreateInstance(type: parameterType);
        }

        if (parameterType.IsInterface || parameterType.IsAbstract)
        {
            return null;
        }

        if (parameterType.GetConstructor(types: Type.EmptyTypes) is not null)
        {
            return Activator.CreateInstance(type: parameterType);
        }

        return null;
    }

    private static bool ContainsSecurityException(Exception exception)
    {
        for (Exception current = exception; current is not null; current = current.InnerException)
        {
            if (current is SecurityException)
            {
                return true;
            }
        }

        return false;
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

    private sealed class UnavailableValue
    {
        internal static UnavailableValue Instance { get; } = new();
    }
}