// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        Type dependency = controllerType
            .GetConstructors()
            .Single()
            .GetParameters()
            .Single()
            .ParameterType;

        // Then
        dependency.FullName
            .Should()
            .Be(expected: expectedServiceContract);

        dependency.IsPublic
            .Should()
            .BeTrue();
    }
}