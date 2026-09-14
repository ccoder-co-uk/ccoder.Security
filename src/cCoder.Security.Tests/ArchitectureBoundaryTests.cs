// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using cCoder.Security.Brokers.Events;
using cCoder.Security.Exposures;
using cCoder.Security.Exposures.EventHandlers;
using cCoder.Security.Models;
using cCoder.Security.Models.Events;
using cCoder.Security.Services.Foundations.Events;
using Microsoft.Extensions.DependencyInjection;
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

    [Fact]
    public void SecurityEventHandlers_WhenConstructed_ShouldOwnEventRegistration()
    {
        // Given
        Type securityAssemblyMarker = typeof(IServiceCollectionExtensions);

        Type formerExposure = securityAssemblyMarker.Assembly.GetType(
            name: "cCoder.Security.Exposures.EventHandlers.SecurityEventHandlers");

        // When
        Type formerEventHandlerService =
            securityAssemblyMarker.Assembly.GetType(
                name: "cCoder.Security.Services.Foundations.Events.IEventHandlerService");

        // Then
        formerExposure.Should()
            .BeNull();

        formerEventHandlerService.Should()
            .BeNull();
    }

    [Fact]
    public void EventHandlers_WhenConfigured_ShouldBeEventSpecificAndOrdered()
    {
        // Given
        IServiceCollection services = new ServiceCollection();
        SecurityConfiguration configuration = new();

        string[] expectedHandlerNames =
        [
            "TenantSetupEventHandlers",
            .. Enum.GetValues<SecurityAccountEventKind>()
                .Select(selector: kind => $"{kind}EventHandlers")
        ];

        // When
        services.AddSecurityWeb(configuration: configuration);

        string[] actualHandlerNames = services
            .Where(predicate: descriptor =>
                descriptor.ServiceType == typeof(ISecurityEventHandlers))
            .Select(selector: descriptor =>
                descriptor.ImplementationType.Name)
            .ToArray();

        // Then
        actualHandlerNames.Should()
            .Equal(expected: expectedHandlerNames);
    }
}