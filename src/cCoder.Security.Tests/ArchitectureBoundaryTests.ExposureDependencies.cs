// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using cCoder.Security.Exposures.Controllers;
using Xunit;

namespace cCoder.Security.Tests;

public sealed partial class ArchitectureBoundaryTests
{
    [Fact]
    public void SetupController_WhenConstructed_ShouldDependOnAggregationServiceContract()
    {
        // Given
        Type setupController = typeof(SetupController);

        // When
        Type dependency = setupController
            .GetConstructors()
            .Single()
            .GetParameters()
            .Single()
            .ParameterType;

        // Then
        dependency.FullName
            .Should()
            .Be(
            expected: "cCoder.Security.Services.Aggregations.Interfaces.IRegistrationAggregationService");

        dependency.IsPublic
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ServiceContracts_WhenDeclared_ShouldNotInheritExposureContracts()
    {
        // Given
        Type securityAssemblyMarker = typeof(IServiceCollectionExtensions);

        Type[] serviceContracts = securityAssemblyMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsInterface
                && type.Namespace?.Contains(
                    value: ".Services.",
                    comparisonType: StringComparison.Ordinal) is true)
            .ToArray();

        // When
        Type[] serviceContractsWithExposureBases = serviceContracts
            .Where(predicate: serviceContract => serviceContract
                .GetInterfaces()
                .Any(predicate: inheritedContract => inheritedContract.Namespace?
                    .Contains(
                        value: ".Exposures",
                        comparisonType: StringComparison.Ordinal) is true))
            .ToArray();

        // Then
        serviceContractsWithExposureBases
            .Should()
            .BeEmpty();
    }
}