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
        string previousConnectionString = Environment.GetEnvironmentVariable(variable: "ENV_ConnectionStrings__SSO");
        string databaseName = $"SSOAcceptanceStartupCacheTests_{Environment.ProcessId}_{Guid.NewGuid():N}";

        string acceptanceConnectionString =
            $"Data Source=.;Initial Catalog={databaseName};MultipleActiveResultSets=True;Trusted_Connection=True;Trust Server Certificate=true";

        // When
        Environment.SetEnvironmentVariable(
variable: "ENV_ConnectionStrings__SSO",
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
            Environment.SetEnvironmentVariable(variable: "ENV_ConnectionStrings__SSO", value: previousConnectionString);
        }
    }
}