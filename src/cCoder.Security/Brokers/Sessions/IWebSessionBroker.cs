// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Brokers.Sessions;

internal interface IWebSessionBroker
{
    void Clear();

    string GetString(string key);

    void Remove(string key);

    void SetString(string key, string value);
}