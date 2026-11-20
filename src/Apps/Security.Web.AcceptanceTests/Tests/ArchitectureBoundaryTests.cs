// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System;
using System.Reflection;
using cCoder.Security.Brokers.Logging;
using FluentAssertions;
using Security.Web.Controllers;
using Xunit;

namespace Security.AcceptanceTests.Tests;

public sealed partial class ArchitectureBoundaryTests
{
    [Theory]
    [InlineData(typeof(HomeController), "Security.Web.Services.Foundations.IHomeService")]
    [InlineData(typeof(CurrentUserController), "Security.Web.Services.Foundations.ICurrentUserService")]
    public void Controller_WhenConstructed_ShouldDependOnServiceContract(
        Type controller,
        string expectedServiceContract)
    {
        // Given
        Type controllerType = controller;

        // When
        ParameterInfo[] dependencies = controllerType
            .GetConstructors()
            .Single()
            .GetParameters()
            .ToArray();

        Type dependency = dependencies
            .Single(predicate: parameter =>
                parameter.ParameterType.FullName == expectedServiceContract)
            .ParameterType;

        // Then
        dependency.FullName
            .Should()
            .Be(expected: expectedServiceContract);

        dependency.IsPublic
            .Should()
            .BeTrue();

        dependencies
            .Where(predicate: parameter => parameter.ParameterType != dependency)
            .Should()
            .ContainSingle()
            .Which.ParameterType.Should()
            .Be(expected: typeof(ILoggingBroker));
    }
}