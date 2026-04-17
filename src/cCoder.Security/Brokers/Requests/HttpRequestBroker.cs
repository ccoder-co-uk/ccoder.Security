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
}

