// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using cCoder.Security.Brokers.Events;
using cCoder.CodeAnalysis.Exposures;
using cCoder.Security.Models;
using cCoder.Security.Services.Foundations.Events;
using Xunit;

namespace cCoder.Security.Tests;

public sealed partial class ArchitectureBoundaryTests
{
    [Fact]
    public void SSOAuthInfoAggregationService_WhenConstructed_ShouldUseEncodingBroker()
    {
        // Given
        Type securityAssemblyMarker = typeof(IServiceCollectionExtensions);

        Type securityAssembly = securityAssemblyMarker.Assembly.GetType(
            name: "cCoder.Security.Services.Aggregations.SSOAuthInfoAggregationService");

        Type encodingBroker = securityAssemblyMarker.Assembly.GetType(
            name: "cCoder.Security.Brokers.Encoding.IEncodingBroker");

        // When
        Type[] constructorDependencies = securityAssembly
            .GetConstructors()
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .ToArray();

        // Then
        encodingBroker.Should()
            .NotBeNull();

        constructorDependencies.Should()
            .Contain(expected: encodingBroker);
    }

    [Fact]
    public void AccountEventService_WhenConstructed_ShouldOwnOneOrdinaryBroker()
    {
        // Given
        Type service = typeof(AccountEventService);

        // When
        Type[] ordinaryBrokerDependencies = service
            .GetConstructors()
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .Where(predicate: dependency =>
                dependency.Name.EndsWith(
                    value: "Broker",
                    comparisonType: StringComparison.Ordinal)
                && !typeof(IUtilityBroker).IsAssignableFrom(c: dependency))
            .ToArray();

        // Then
        ordinaryBrokerDependencies.Should()
            .ContainSingle()
            .Which.Should()
            .Be(expected: typeof(IAccountEventBroker));
    }

}