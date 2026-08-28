// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using cCoder.Security.Data.EF.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.Security.Tests;

public sealed partial class SecurityConfigurationTests
{
    [Fact]
    public void SecurityConfiguration_ShouldNotOwnPersistenceConfiguration()
    {
        // Given
        Type configurationType = typeof(SecurityConfiguration);

        // When
        System.Reflection.PropertyInfo connectionStringProperty =
            configurationType.GetProperty(name: "ConnectionString");

        // Then
        connectionStringProperty.Should()
            .BeNull();
    }

    [Fact]
    public void AddSecurityWeb_ShouldNotRegisterSecurityDataServices()
    {
        // Given
        IServiceCollection services = new ServiceCollection();
        SecurityConfiguration configuration = new();

        typeof(SecurityConfiguration)
            .GetProperty(name: "ConnectionString")
            ?.SetValue(
                obj: configuration,
                value: "Server=(local);Database=security;");

        // When
        services.AddSecurityWeb(configuration: configuration);

        // Then
        services.Should()
            .NotContain(predicate: descriptor =>
                descriptor.ServiceType == typeof(ISecurityDbContextFactory));
    }

    [Fact]
    public void SecurityDataConfiguration_ShouldOwnRuntimeAndMigrationConnections()
    {
        // Given
        const string runtimeConnection = "runtime";
        const string migrationConnection = "migration";

        // When
        SecurityDataConfiguration configuration = new()
        {
            ConnectionString = runtimeConnection,
            AdminConnectionString = migrationConnection
        };

        // Then
        configuration.ConnectionString
            .Should()
            .Be(expected: runtimeConnection);

        configuration.AdminConnectionString
            .Should()
            .Be(expected: migrationConnection);
    }
}