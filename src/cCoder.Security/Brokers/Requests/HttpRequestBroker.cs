// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;

namespace cCoder.Security.Brokers.Requests;

internal class HttpRequestBroker(HttpRequest request)
    : IHttpRequestBroker
{
    public bool HasHeader(string headerValue) =>
        request?.Headers.ContainsKey(key: headerValue) ?? false;

    public string Header(string key) =>
        request?.Headers.ContainsKey(key: key) ?? false
            ? request?.Headers[key].ToString()
            : null;

    public string GetRequestDomain()
    {
        string forwardedHost = Header(key: "X-Forwarded-Host");

        if (!string.IsNullOrWhiteSpace(value: forwardedHost))
            return forwardedHost;

        return request?.Host.Host;
    }
}