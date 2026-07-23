// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Security.AcceptanceTests.Tests;

public sealed partial class BaselineTests
{
    private async Task<JsonElement> GetBaselineAsync()
    {
        using WebApplicationFactory<AcceptanceHost> factory = new();
        using HttpClient client = factory.CreateClient();
        using HttpResponseMessage response = await client.GetAsync(requestUri: "/Api/Security/Baseline");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonDocument.Parse(json: content)
                   .RootElement.Clone();
    }
}