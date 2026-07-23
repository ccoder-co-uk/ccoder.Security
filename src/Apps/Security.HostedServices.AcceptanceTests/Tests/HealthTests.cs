// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        HttpResponseMessage response = await client.GetAsync(requestUri: "/");

        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(expected: "cCoder.Security Hosted Services");
        content.Should().Contain(expected: "TokenCleaner");
        content.Should().NotContain(unexpected: "tenant_setup");
        content.Should().NotContain(unexpected: "Hosted event listeners");
    }

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
}