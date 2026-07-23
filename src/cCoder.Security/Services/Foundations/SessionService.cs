// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Serialization;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Http;

namespace cCoder.Security.Services.Foundations;

internal class SessionService(ISession session, ISerializationBroker serilizationBroker)
    : ISessionService
{
    public void SetString(string key, string value)
    {
        if (value is null)
            RemoveKey(key: key);
        else
            session.SetString(key: key, value: value);
    }

    public void Clear() =>
        session.Clear();

    public string GetString(string key)
    {
        try
        {
            return session?.GetString(key: key);
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }

    public SSOUser GetUser()
    {
        string userJson = GetString(key: "ssoUser");

        return !string.IsNullOrEmpty(value: userJson)
            ? serilizationBroker.Deserialize<SSOUser>(input: userJson)
            : null;
    }

    public void SetUser(SSOUser user)
    {
        if (user != null)
            session?.SetString(key: "ssoUser", value: System.Text.Json.JsonSerializer.Serialize(user));
        else
            session?.Remove(key: "ssoUser");
    }

    public void RemoveKey(string key) =>
        session.Remove(key: key);
}