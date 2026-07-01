using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Security.HostedServices.AcceptanceTests.Tests;

public class HealthTests
{
    [Fact]
    public async Task ShouldReturnHostedServicesReportForGetRoot()
    {
        using WebApplicationFactory<AcceptanceHost> factory = new();
        using HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("cCoder.Security Hosted Services");
        content.Should().Contain("TokenCleaner");
        content.Should().NotContain("tenant_setup");
        content.Should().NotContain("Hosted event listeners");
    }

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
}
