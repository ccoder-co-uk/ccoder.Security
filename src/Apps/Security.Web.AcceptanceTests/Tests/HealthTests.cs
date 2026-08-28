// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using cCoder.Security.Models;
using Security.AcceptanceTests;
using Xunit;

namespace Security.AcceptanceTests.Tests;

public partial class HealthTests
{
    [Fact]
    public void ShouldExposeAppConfigurationForRequiredDomains()
    {
        // Given
        const string typeName =
            "Security.Web.Models.AppConfiguration, Security.Web";

        // When
        Type configurationType = Type.GetType(typeName: typeName);

        // Then
        configurationType.Should()
            .NotBeNull();

        configurationType.GetProperty(name: "Security")
            .PropertyType.Should()
            .Be(expected: typeof(SecurityConfiguration));

        configurationType.GetProperty(name: "SecurityData")
            .PropertyType.Should()
            .Be(expected: typeof(SecurityDataConfiguration));
    }

    [Fact]
    public async Task ShouldReturnHealthyForGetHealth()
    {
        // Given
        using SecurityWebApplicationFactory factory = new();
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
        using SecurityWebApplicationFactory factory = new();
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
        using SecurityWebApplicationFactory factory = new();
        using HttpClient client = factory.CreateClient();

        // When
        HttpResponseMessage response =
            await client.GetAsync(requestUri: "/CurrentUser");

        // Then
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();

        content.Should()
            .Be(expected: "Guest");
    }
}