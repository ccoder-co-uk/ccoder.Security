// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;

namespace cCoder.Security.Brokers.Sessions;

internal sealed class WebSessionBroker(ISession session) : IWebSessionBroker
{
    public void Clear() =>
        session.Clear();

    public string GetString(string key) =>
        session.GetString(key: key);

    public void Remove(string key) =>
        session.Remove(key: key);

    public void SetString(string key, string value) =>
        session.SetString(key: key, value: value);
}