// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Security.AcceptanceTests;
using Xunit;

namespace Security.AcceptanceTests.Tests;

public partial class HealthTests
{
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