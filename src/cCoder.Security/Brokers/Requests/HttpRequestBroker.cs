using Microsoft.AspNetCore.Http;

namespace cCoder.Security.Brokers.Requests;
internal class HttpRequestBroker(HttpRequest request) 
    : IHttpRequestBroker
{
    public bool HasHeader(string headerValue) =>
        request?.Headers.ContainsKey(headerValue) ?? false;

    public string Header(string key) =>
        request?.Headers.ContainsKey(key) ?? false
            ? request?.Headers[key].ToString()
            : null;

    public string GetRequestDomain()
    {
        string forwardedHost = Header("X-Forwarded-Host");

        if (!string.IsNullOrWhiteSpace(forwardedHost))
            return forwardedHost;

        return request?.Host.Host;
    }
}

