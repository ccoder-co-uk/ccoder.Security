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

        HttpResponseMessage response = await client.GetAsync("/Health");

        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }

    [Fact]
    public async Task ShouldServeSecurityUiForGetRoot()
    {
        using WebApplicationFactory<AcceptanceHost> factory = new();
        using HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("cCoder.Security");
    }

    [Fact]
    public async Task ShouldInitialiseDatabaseBackedSessionCacheForCurrentUser()
    {
        string previousConnectionString = Environment.GetEnvironmentVariable("ENV_ConnectionStrings__SSO");
        string databaseName = $"SSOStartupCacheTests_{Environment.ProcessId}_{Guid.NewGuid():N}";

        Environment.SetEnvironmentVariable(
            "ENV_ConnectionStrings__SSO",
            $"Data Source=.;Initial Catalog={databaseName};MultipleActiveResultSets=True;Trusted_Connection=True;Trust Server Certificate=true");

        try
        {
            using WebApplicationFactory<AcceptanceHost> factory = new();
            using HttpClient client = factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync("/CurrentUser");

            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Guest");
        }
        finally
        {
            Environment.SetEnvironmentVariable("ENV_ConnectionStrings__SSO", previousConnectionString);
        }
    }
}
