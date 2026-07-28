// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Security.AcceptanceTests;
using Xunit;

namespace Security.AcceptanceTests.Tests;

public partial class HealthTests
{
    [Fact]
    public async Task ShouldReturnHealthyForGetHealth()
    {
        // Given
        using WebApplicationFactory<AcceptanceHost> factory = new();
        using HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync(requestUri: "/Health");

        response.EnsureSuccessStatusCode();
        // When
        string content = await response.Content.ReadAsStringAsync();

        // Then
        content.Should()
            .Be(expected: "Healthy");
    }

    [Fact]
    public async Task ShouldServeSecurityUiForGetRoot()
    {
        // Given
        using WebApplicationFactory<AcceptanceHost> factory = new();
        using HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync(requestUri: "/");

        response.EnsureSuccessStatusCode();
        // When
        string content = await response.Content.ReadAsStringAsync();

        // Then
        content.Should()
            .Contain(expected: "cCoder.Security");
    }

    [Fact]
    public async Task ShouldInitialiseDatabaseBackedSessionCacheForCurrentUser()
    {
        // Given
        string previousConnectionString = Environment.GetEnvironmentVariable(variable: "Security__ConnectionString");

        Microsoft.Data.SqlClient.SqlConnectionStringBuilder builder =
            new(previousConnectionString);

        builder.InitialCatalog =
            $"{builder.InitialCatalog}-acceptance-{Guid.NewGuid():N}";

        string acceptanceConnectionString = builder.ConnectionString;

        // When
        Environment.SetEnvironmentVariable(
variable: "Security__ConnectionString",
value: acceptanceConnectionString);

        // Then
        try
        {
            using WebApplicationFactory<AcceptanceHost> factory = new();
            using HttpClient client = factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync(requestUri: "/CurrentUser");

            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();

            content.Should()
                .Be(expected: "Guest");
        }
        finally
        {
            SecurityWebApplicationFactoryExtensions.DropDatabaseForTesting(connectionString: acceptanceConnectionString);
            Environment.SetEnvironmentVariable(variable: "Security__ConnectionString", value: previousConnectionString);
        }
    }
}