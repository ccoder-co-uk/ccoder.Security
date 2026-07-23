// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Security.AcceptanceTests;
using Xunit;

namespace Security.AcceptanceTests.Tests;

public class HealthTests
{
    [Fact]
    public async Task ShouldReturnHealthyForGetHealth()
    {
        using WebApplicationFactory<AcceptanceHost> factory = new();
        using HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync(requestUri: "/Health");

        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        content.Should().Be(expected: "Healthy");
    }

    [Fact]
    public async Task ShouldServeSecurityUiForGetRoot()
    {
        using WebApplicationFactory<AcceptanceHost> factory = new();
        using HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync(requestUri: "/");

        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(expected: "cCoder.Security");
    }

    [Fact]
    public async Task ShouldInitialiseDatabaseBackedSessionCacheForCurrentUser()
    {
        string previousConnectionString = Environment.GetEnvironmentVariable(variable: "ENV_ConnectionStrings__SSO");
        string databaseName = $"SSOAcceptanceStartupCacheTests_{Environment.ProcessId}_{Guid.NewGuid():N}";

        string acceptanceConnectionString =
            $"Data Source=.;Initial Catalog={databaseName};MultipleActiveResultSets=True;Trusted_Connection=True;Trust Server Certificate=true";

        Environment.SetEnvironmentVariable(
variable: "ENV_ConnectionStrings__SSO",
value: acceptanceConnectionString);

        try
        {
            using WebApplicationFactory<AcceptanceHost> factory = new();
            using HttpClient client = factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync(requestUri: "/CurrentUser");

            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            content.Should().Be(expected: "Guest");
        }
        finally
        {
            SecurityWebApplicationFactoryExtensions.DropDatabaseForTesting(connectionString: acceptanceConnectionString);
            Environment.SetEnvironmentVariable(variable: "ENV_ConnectionStrings__SSO", value: previousConnectionString);
        }
    }
}