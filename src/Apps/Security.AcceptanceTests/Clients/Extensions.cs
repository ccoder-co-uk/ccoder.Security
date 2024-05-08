using cCoder.Security.Objects.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Text;

namespace Security.AcceptanceTests.Clients;

public static class Extensions
{
    public static JsonSerializerSettings GetJsonSettings() => new()
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
        JsonConvert.SerializeObject(o, Formatting.None, GetJsonSettings());

    /// <summary>
    /// Adds authorization information to the client by making an auth call with the given credentials
    /// </summary>
    /// <param name="client">The HttpClient to attach the authorization information to</param>
    /// <param name="user">The username to use for authentication</param>
    /// <param name="pass">The password to use for authentication</param>
    /// <returns>An authenticated HttpClient</returns>
    public static async Task<Token> Authenticate(this HttpClient client, string user, string pass)
    {
        var auth = new { User = user, Pass = pass };

        HttpResponseMessage response = await client.PostAsync("Api/Account/Login", new StringContent(auth.ToJson(), Encoding.UTF8, "application/json"));
        _ = response.EnsureSuccessStatusCode();
        Token token = await response.Content.ReadAsAsync<Token>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Id);
        return token;
    }

    /// <summary>
    /// Fetches a collection of data items of type T from the API
    /// </summary>
    /// <typeparam name="TResult">Expected result type</typeparam>
    /// <param name="client">HttpClient instance</param>
    /// <param name="query">Path to API call</param>
    /// <returns>Task that resolves our Result set as an ODataCollection&gt;T&lt;</returns>
    public static async Task<IEnumerable<T>> GetODataCollection<T>(this HttpClient client, string query)
    {
        List<T> results = [];
        int page = 0;
        int batchSize = 1000;
        string fullQuery = query + (query.Contains('?') ? $"&$skip={page * batchSize}&$top={batchSize}" : $"?$skip={page * batchSize}&$top={batchSize}");

        ODataCollection<T> batch = await client.GetAsync<ODataCollection<T>>(fullQuery);

        while (batch?.Value?.Any() ?? false)
        {
            results.AddRange(batch.Value);
            page++;
            fullQuery = query + (query.Contains('?') ? $"&$skip={page * batchSize}&$top={batchSize}" : $"?$skip={page * batchSize}&$top={batchSize}");
            batch = await client.GetAsync<ODataCollection<T>>(fullQuery);
        }

        return results;
    }

    /// <summary>
    /// Get a string from the API
    /// </summary>
    /// <param name="client">HttpClient instance</param>
    /// <param name="query">Path to API call</param>
    /// <returns>The requested response as a T</returns>
    public static async Task<T> GetAsync<T>(this HttpClient client, string query)
    {
        HttpResponseMessage result = await client.GetAsync(query);
        _ = result.EnsureSuccessStatusCode();
        return await result.Content.ReadAsAsync<T>();
    }

    public static async Task<T> ReadAsAsync<T>(this HttpContent content) =>
        JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync(), GetJsonSettings());
}
