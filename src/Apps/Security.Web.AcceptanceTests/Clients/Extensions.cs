// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using cCoder.Security.Models.Entities;
using Security.AcceptanceTests.Tests.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Security.AcceptanceTests.Clients;

public static class Extensions
{
    public static JsonSerializerOptions GetJsonSettings() =>
        new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            MaxDepth = 4
        };

    public static string ToJson(this object o) =>
        JsonSerializer.Serialize(value: o, options: GetJsonSettings());

    public static async Task<Token> Authenticate(this HttpClient client, string user, string pass)
    {
        var auth = new { User = user, Pass = pass };

        HttpResponseMessage response = await client.PostAsync(requestUri: "Api/Account/Login", content: new StringContent(auth.ToJson(), Encoding.UTF8, "application/json"));
        _ = response.EnsureSuccessStatusCode();
        Token token = await response.Content.ReadTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Id);
        return token;
    }

    public static async Task<IEnumerable<SSOUser>> GetSSOUsersAsync(
        this HttpClient client,
        string query)
    {
        List<SSOUser> results = [];
        int page = 0;
        int batchSize = 1000;
        string fullQuery = query + (query.Contains(value: '?') ? $"&$skip={page * batchSize}&$top={batchSize}" : $"?$skip={page * batchSize}&$top={batchSize}");

        ODataCollection<SSOUser> batch =
            await client.GetSSOUserCollectionAsync(query: fullQuery);

        while (batch?.Value?.Any() ?? false)
        {
            results.AddRange(collection: batch.Value);
            page++;
            fullQuery = query + (query.Contains(value: '?') ? $"&$skip={page * batchSize}&$top={batchSize}" : $"?$skip={page * batchSize}&$top={batchSize}");

            batch = await client.GetSSOUserCollectionAsync(
                query: fullQuery);
        }

        return results;
    }

    private static async Task<ODataCollection<SSOUser>>
        GetSSOUserCollectionAsync(
            this HttpClient client,
            string query)
    {
        HttpResponseMessage result = await client.GetAsync(requestUri: query);
        _ = result.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<ODataCollection<SSOUser>>(
            json: await result.Content.ReadAsStringAsync(),
            options: GetJsonSettings());
    }

    public static async Task<Token> ReadTokenAsync(
        this HttpContent content) =>
        JsonSerializer.Deserialize<Token>(
            json: await content.ReadAsStringAsync(),
            options: GetJsonSettings());

    public static async Task<SSOUser> ReadSSOUserAsync(
        this HttpContent content) =>
        JsonSerializer.Deserialize<SSOUser>(
            json: await content.ReadAsStringAsync(),
            options: GetJsonSettings());

    public static async Task<RegistrationResult> ReadRegistrationResultAsync(
        this HttpContent content) =>
        JsonSerializer.Deserialize<RegistrationResult>(
            json: await content.ReadAsStringAsync(),
            options: GetJsonSettings());
}