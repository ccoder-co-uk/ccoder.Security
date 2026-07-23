// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Security.AcceptanceTests.Tests.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Text;

namespace Security.AcceptanceTests.Clients;

public static class Extensions
{
    public static JsonSerializerSettings GetJsonSettings() =>
        new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true },
            MaxDepth = 4
        };

    public static string ToJson(this object o) =>
        JsonConvert.SerializeObject(value: o, formatting: Formatting.None, settings: GetJsonSettings());

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

        return JsonConvert.DeserializeObject<ODataCollection<SSOUser>>(
            value: await result.Content.ReadAsStringAsync(),
            settings: GetJsonSettings());
    }

    public static async Task<Token> ReadTokenAsync(
        this HttpContent content) =>
        JsonConvert.DeserializeObject<Token>(
            value: await content.ReadAsStringAsync(),
            settings: GetJsonSettings());

    public static async Task<SSOUser> ReadSSOUserAsync(
        this HttpContent content) =>
        JsonConvert.DeserializeObject<SSOUser>(
            value: await content.ReadAsStringAsync(),
            settings: GetJsonSettings());

    public static async Task<RegistrationResult> ReadRegistrationResultAsync(
        this HttpContent content) =>
        JsonConvert.DeserializeObject<RegistrationResult>(
            value: await content.ReadAsStringAsync(),
            settings: GetJsonSettings());
}