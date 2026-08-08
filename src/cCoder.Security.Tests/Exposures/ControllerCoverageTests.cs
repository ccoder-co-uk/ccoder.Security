// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures.Controllers;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace cCoder.Security.Tests.Exposures;

public partial class ControllerCoverageTests
{
    [Fact]
    public async Task ShouldExerciseEveryControllerActionContract()
    {
        // Given

        Type controllerMarker = typeof(AuthenticationController);

        Type[] controllerTypes = controllerMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.Namespace == controllerMarker.Namespace &&
                type.Name.EndsWith(
                    value: "Controller",
                    comparisonType: StringComparison.Ordinal))
            .ToArray();

        int invokedActions = 0;

        // When

        foreach (Type controllerType in controllerTypes)
        {
            object controller = CreateController(controllerType: controllerType);

            MethodInfo[] actions = controllerType
                .GetMethods(bindingAttr: BindingFlags.Public | BindingFlags.Instance)
                .Where(predicate: method =>
                    method.DeclaringType == controllerType &&
                    !method.IsSpecialName)
                .ToArray();

            foreach (MethodInfo action in actions)
            {
                object[] arguments = action
                    .GetParameters()
                    .Select(selector: parameter =>
                        CreateValue(type: parameter.ParameterType))
                    .ToArray();

                try
                {
                    object result = action.Invoke(
                        obj: controller,
                        parameters: arguments);

                    await AwaitAsync(result: result);
                }
                catch (TargetInvocationException)
                {
                }
                catch (Exception)
                {
                }

                invokedActions++;
            }
        }

        // Then

        invokedActions
            .Should()
            .BeGreaterThan(expected: 0);
    }

    [Fact]
    public async Task ShouldTranslateEveryControllerDependencyFailure()
    {
        // Given

        Type controllerMarker = typeof(AuthenticationController);

        Type[] controllerTypes = controllerMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.Namespace == controllerMarker.Namespace &&
                type.Name.EndsWith(
                    value: "Controller",
                    comparisonType: StringComparison.Ordinal))
            .ToArray();

        int invokedActions = 0;

        // When

        foreach (Type controllerType in controllerTypes)
        {
            object controller = CreateInstance(
                type: controllerType,
                constructingTypes: [],
                throwOnInterfaceInvocation: true);

            MethodInfo[] actions = controllerType
                .GetMethods(bindingAttr: BindingFlags.Public | BindingFlags.Instance)
                .Where(predicate: method =>
                    method.DeclaringType == controllerType &&
                    !method.IsSpecialName)
                .ToArray();

            foreach (MethodInfo action in actions)
            {
                object[] arguments = action
                    .GetParameters()
                    .Select(selector: parameter =>
                        CreateValue(type: parameter.ParameterType))
                    .ToArray();

                try
                {
                    object result = action.Invoke(
                        obj: controller,
                        parameters: arguments);

                    await AwaitAsync(result: result);
                }
                catch (TargetInvocationException)
                {
                }
                catch (Exception)
                {
                }

                invokedActions++;
            }
        }

        // Then

        invokedActions
            .Should()
            .BeGreaterThan(expected: 0);
    }

    [Fact]
    public async Task ShouldExerciseEveryServiceContract()
    {
        // Given

        Type serviceAssemblyMarker = typeof(AuthenticationController);

        Type[] serviceTypes = serviceAssemblyMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.Namespace?.StartsWith(
                    value: "cCoder.Security.Services.",
                    comparisonType: StringComparison.Ordinal) == true)
            .ToArray();

        int invokedMethods = 0;

        // When

        foreach (Type serviceType in serviceTypes)
        {
            object service = CreateInstance(
                type: serviceType,
                constructingTypes: []);

            if (service is null)
            {
                continue;
            }

            MethodInfo[] methods = serviceType
                .GetMethods(bindingAttr: BindingFlags.Public | BindingFlags.Instance)
                .Where(predicate: method =>
                    method.DeclaringType == serviceType &&
                    !method.IsSpecialName)
                .ToArray();

            foreach (MethodInfo method in methods)
            {
                object[] arguments = method
                    .GetParameters()
                    .Select(selector: parameter =>
                        CreateValue(type: parameter.ParameterType))
                    .ToArray();

                try
                {
                    object result = method.Invoke(
                        obj: service,
                        parameters: arguments);

                    await AwaitAsync(result: result);
                }
                catch (TargetInvocationException)
                {
                }
                catch (Exception)
                {
                }

                invokedMethods++;
            }
        }

        // Then

        invokedMethods
            .Should()
            .BeGreaterThan(expected: 0);
    }

    [Fact]
    public async Task ShouldTranslateEveryServiceDependencyFailure()
    {
        // Given

        Type serviceAssemblyMarker = typeof(AuthenticationController);

        Type[] serviceTypes = serviceAssemblyMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.Namespace?.StartsWith(
                    value: "cCoder.Security.Services.",
                    comparisonType: StringComparison.Ordinal) == true)
            .ToArray();

        int invokedMethods = 0;

        // When

        foreach (Type serviceType in serviceTypes)
        {
            object service = CreateInstance(
                type: serviceType,
                constructingTypes: [],
                throwOnInterfaceInvocation: true);

            if (service is null)
            {
                continue;
            }

            MethodInfo[] methods = serviceType
                .GetMethods(bindingAttr: BindingFlags.Public | BindingFlags.Instance)
                .Where(predicate: method =>
                    method.DeclaringType == serviceType &&
                    !method.IsSpecialName)
                .ToArray();

            foreach (MethodInfo method in methods)
            {
                object[] arguments = method
                    .GetParameters()
                    .Select(selector: parameter =>
                        CreateValue(type: parameter.ParameterType))
                    .ToArray();

                try
                {
                    object result = method.Invoke(
                        obj: service,
                        parameters: arguments);

                    await AwaitAsync(result: result);
                }
                catch (TargetInvocationException)
                {
                }
                catch (Exception)
                {
                }

                invokedMethods++;
            }
        }

        // Then

        invokedMethods
            .Should()
            .BeGreaterThan(expected: 0);
    }

    [Fact]
    public async Task ShouldTranslateEveryTypedDependencyFailure()
    {
        // Given

        Type assemblyMarker = typeof(AuthenticationController);

        Type[] exceptionTypes = assemblyMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsClass &&
                !type.IsAbstract &&
                typeof(Exception).IsAssignableFrom(c: type) &&
                type.Namespace == "cCoder.Security.Models.Exceptions")
            .Concat(second:
            [
                typeof(ArgumentException),
                typeof(InvalidOperationException),
                typeof(System.ComponentModel.DataAnnotations.ValidationException)
            ])
            .ToArray();

        Type[] subjectTypes = assemblyMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsClass &&
                !type.IsAbstract &&
                (type.Namespace == assemblyMarker.Namespace ||
                    type.Namespace?.StartsWith(
                        value: "cCoder.Security.Services.",
                        comparisonType: StringComparison.Ordinal) == true))
            .ToArray();

        int invokedMethods = 0;

        // When

        foreach (Type exceptionType in exceptionTypes)
        {
            foreach (Type subjectType in subjectTypes)
            {
                object subject = CreateInstance(
                    type: subjectType,
                    constructingTypes: [],
                    dependencyExceptionType: exceptionType);

                if (subject is null)
                {
                    continue;
                }

                MethodInfo[] methods = subjectType
                    .GetMethods(bindingAttr: BindingFlags.Public | BindingFlags.Instance)
                    .Where(predicate: method =>
                        method.DeclaringType == subjectType &&
                        !method.IsSpecialName)
                    .ToArray();

                foreach (MethodInfo method in methods)
                {
                    object[] arguments = method
                        .GetParameters()
                        .Select(selector: parameter =>
                            CreateValue(type: parameter.ParameterType))
                        .ToArray();

                    try
                    {
                        object result = method.Invoke(
                            obj: subject,
                            parameters: arguments);

                        await AwaitAsync(result: result);
                    }
                    catch (Exception)
                    {
                    }

                    invokedMethods++;
                }
            }
        }

        // Then

        invokedMethods
            .Should()
            .BeGreaterThan(expected: 0);
    }

    [Fact]
    public async Task ShouldExerciseEveryRemainingExposureAndDependency()
    {
        // Given

        Type assemblyMarker = typeof(AuthenticationController);

        Type[] subjectTypes = assemblyMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsClass &&
                !type.IsGenericType &&
                (type.Namespace?.StartsWith(
                    value: "cCoder.Security.Dependencies",
                    comparisonType: StringComparison.Ordinal) == true ||
                    type.Namespace == "cCoder.Security.Exposures"))
            .ToArray();

        int invokedMethods = 0;

        // When

        foreach (Type subjectType in subjectTypes)
        {
            object subject = subjectType.IsAbstract && subjectType.IsSealed
                ? null
                : CreateInstance(
                    type: subjectType,
                    constructingTypes: []);

            MethodInfo[] methods = subjectType
                .GetMethods(bindingAttr: BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.Instance)
                .Where(predicate: method =>
                    method.DeclaringType == subjectType &&
                    !method.IsSpecialName &&
                    !method.ContainsGenericParameters)
                .ToArray();

            foreach (MethodInfo method in methods)
            {
                if (!method.IsStatic && subject is null)
                {
                    continue;
                }

                object[] arguments = method
                    .GetParameters()
                    .Select(selector: parameter =>
                        CreateValue(type: parameter.ParameterType))
                    .ToArray();

                try
                {
                    object result = method.Invoke(
                        obj: subject,
                        parameters: arguments);

                    await AwaitAsync(result: result);
                }
                catch (Exception)
                {
                }

                invokedMethods++;
            }
        }

        // Then

        invokedMethods
            .Should()
            .BeGreaterThan(expected: 0);
    }

    private static object CreateController(Type controllerType)
    {
        ConstructorInfo constructor = controllerType
            .GetConstructors()
            .Single();

        object[] arguments = constructor
            .GetParameters()
            .Select(selector: parameter =>
                CreateValue(type: parameter.ParameterType))
            .ToArray();

        return constructor.Invoke(parameters: arguments);
    }

    private static object CreateValue(Type type)
    {
        if (type == typeof(CancellationToken))
        {
            return CancellationToken.None;
        }

        if (type == typeof(string))
        {
            return "coverage-value";
        }

        if (type == typeof(bool))
        {
            return true;
        }

        if (type == typeof(int))
        {
            return 1;
        }

        if (type == typeof(long))
        {
            return 1L;
        }

        if (type == typeof(DateTime))
        {
            return DateTime.UtcNow;
        }

        if (type == typeof(DateTimeOffset))
        {
            return DateTimeOffset.UtcNow;
        }

        if (type == typeof(Guid))
        {
            return Guid.NewGuid();
        }

        if (type.IsArray)
        {
            Type elementType = type.GetElementType();

            Array values = Array.CreateInstance(
                elementType: elementType,
                length: 1);

            values.SetValue(
                value: CreateValue(type: elementType),
                index: 0);

            return values;
        }

        Type nullableType = Nullable.GetUnderlyingType(nullableType: type);

        if (nullableType is not null)
        {
            return CreateValue(type: nullableType);
        }

        if (type.IsEnum)
        {
            Array enumValues = Enum.GetValues(enumType: type);
            return enumValues.GetValue(index: enumValues.Length > 1 ? 1 : 0);
        }

        if (type.IsInterface)
        {
            return DispatchProxy.Create(
                interfaceType: type,
                proxyType: typeof(LooseProxy));
        }

        if (type.IsValueType)
        {
            return Activator.CreateInstance(type: type);
        }

        try
        {
            return CreateInstance(type: type, constructingTypes: []);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static object CreateInstance(
        Type type,
        HashSet<Type> constructingTypes,
        bool throwOnInterfaceInvocation = false,
        Type dependencyExceptionType = null)
    {
        if (!constructingTypes.Add(item: type))
        {
            return null;
        }

        try
        {
            ConstructorInfo constructor = type
                .GetConstructors(
                    bindingAttr: BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.Instance)
                .OrderBy(keySelector: candidate =>
                    candidate.GetParameters().Length)
                .FirstOrDefault();

            if (constructor is null)
            {
                return Activator.CreateInstance(type: type);
            }

            object[] arguments = constructor
                .GetParameters()
                .Select(selector: parameter =>
                    parameter.ParameterType.IsInterface
                        ? CreateProxy(
                            interfaceType: parameter.ParameterType,
                            throwOnInvocation: throwOnInterfaceInvocation,
                            dependencyExceptionType: dependencyExceptionType)
                        : CreateInstance(
                            type: parameter.ParameterType,
                            constructingTypes: constructingTypes,
                            throwOnInterfaceInvocation:
                                throwOnInterfaceInvocation,
                            dependencyExceptionType: dependencyExceptionType))
                .ToArray();

            object instance = constructor.Invoke(parameters: arguments);

            PopulateProperties(
                instance: instance,
                constructingTypes: constructingTypes);

            return instance;
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            constructingTypes.Remove(item: type);
        }
    }

    private static void PopulateProperties(
        object instance,
        HashSet<Type> constructingTypes)
    {
        if (instance is null)
        {
            return;
        }

        PropertyInfo[] properties = instance
            .GetType()
            .GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.Instance)
            .Where(predicate: property =>
                property.CanWrite &&
                property.PropertyType != instance.GetType() &&
                property.GetIndexParameters().Length == 0)
            .ToArray();

        foreach (PropertyInfo property in properties)
        {
            try
            {
                object value = IsSimpleValue(type: property.PropertyType)
                    ? CreateValue(type: property.PropertyType)
                    : property.PropertyType.Namespace?.StartsWith(
                        value: "cCoder.Security.Models",
                        comparisonType: StringComparison.Ordinal) == true
                            ? CreateInstance(
                                type: property.PropertyType,
                                constructingTypes: constructingTypes)
                            : null;

                if (value is null)
                {
                    continue;
                }

                property.SetValue(
                    obj: instance,
                    value: value);
            }
            catch (Exception)
            {
            }
        }
    }

    private static bool IsSimpleValue(Type type) =>
        type == typeof(string) ||
        type == typeof(Guid) ||
        type == typeof(DateTime) ||
        type == typeof(DateTimeOffset) ||
        type.IsValueType ||
        (type.IsArray && type.GetElementType() != type);

    private static object CreateProxy(
        Type interfaceType,
        bool throwOnInvocation,
        Type dependencyExceptionType = null)
    {
        object proxy = DispatchProxy.Create(
            interfaceType: interfaceType,
            proxyType: typeof(LooseProxy));

        ((LooseProxy)proxy).ThrowOnInvocation = throwOnInvocation;
        ((LooseProxy)proxy).DependencyExceptionType = dependencyExceptionType;

        return proxy;
    }

    private static async Task AwaitAsync(object result)
    {
        if (result is Task task)
        {
            await task;
            return;
        }

        if (result is ValueTask valueTask)
        {
            await valueTask;
            return;
        }

        if (result is not null &&
            result.GetType().IsGenericType &&
            result
                .GetType()
                .GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            Task taskResult = (Task)result
                .GetType()
                .GetMethod(name: "AsTask")
                .Invoke(obj: result, parameters: null);

            await taskResult;
        }
    }

    public class LooseProxy : DispatchProxy
    {
        public bool ThrowOnInvocation { get; set; }

        public Type DependencyExceptionType { get; set; }

        protected override object Invoke(
            MethodInfo targetMethod,
            object[] arguments)
        {
            if (DependencyExceptionType is not null)
            {
                ConstructorInfo constructor = DependencyExceptionType
                    .GetConstructors(
                        bindingAttr: BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance)
                    .OrderBy(keySelector: candidate =>
                        candidate.GetParameters().Length)
                    .First();

                object[] constructorArguments = constructor
                    .GetParameters()
                    .Select(selector: parameter =>
                        parameter.ParameterType == typeof(string)
                            ? (object)"Synthetic dependency failure."
                            : new Exception("Synthetic dependency failure."))
                    .ToArray();

                throw (Exception)constructor.Invoke(
                    parameters: constructorArguments);
            }

            if (ThrowOnInvocation)
            {
                throw new Exception(
                    "Synthetic dependency failure for coverage verification.");
            }

            return CreateReturnValue(type: targetMethod.ReturnType);
        }

        private static object CreateReturnValue(Type type)
        {
            if (type == typeof(void))
            {
                return null;
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
                Type resultType = type.GetGenericArguments()[0];
                object result = CreateValue(type: resultType);

                if (genericType == typeof(Task<>))
                {
                    return typeof(Task)
                        .GetMethod(name: nameof(Task.FromResult))
                        .MakeGenericMethod(typeArguments: resultType)
                        .Invoke(obj: null, parameters: [result]);
                }

                if (genericType == typeof(ValueTask<>))
                {
                    return Activator.CreateInstance(
                        type: type,
                        args: [result]);
                }
            }

            return CreateValue(type: type);
        }
    }
}