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
        request?.Headers[key].ToString();

    public string RequestHost() =>
        request?.Host.Host;
}