// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using FluentAssertions;
using Xunit;

namespace cCoder.Security.Tests;

public sealed partial class SecurityConfigurationTests
{
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