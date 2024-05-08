using cCoder.Security.Objects.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Text;

namespace cCoder.Security.Objects;

public static class HttpClientExtensions
{
    /// <summary>
    /// Adds authorization information to the client by making an auth call with the given credentials
    /// </summary>
    /// <param name="client">The HttpClient to attach the authorization information to</param>
    /// <param name="user">The username to use for authentication</param>
    /// <param name="pass">The password to use for authentication</param>
    /// <returns>An authenticated HttpClient</returns>
    public static async Task<Token> Authenticate(this HttpClient client, string user, string pass)
    {
        try
        {
            var auth = new { User = user, Pass = pass };
            HttpResponseMessage response = await client.PostAsync("Account/Login", new StringContent(Json(auth), Encoding.UTF8, "application/json"));
            _ = response.EnsureSuccessStatusCode();
            Token token = await ReadAsAsync<Token>(response.Content);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Id);
            return token;
        }
        catch { /* if we get here the server returned a json response but it wasn't a token, it was more than likely an auth failure. */ }
        return null;
    }

    public static async Task<T> ReadAsAsync<T>(HttpContent content) 
        => JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());

    static string Json(object source) => JsonConvert.SerializeObject(source, new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        TypeNameHandling = TypeNameHandling.None,
        Formatting = Formatting.None,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        NullValueHandling = NullValueHandling.Ignore,
        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
        ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true },
        MaxDepth = 4
    });
}