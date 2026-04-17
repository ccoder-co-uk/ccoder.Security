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
            RemoveKey(key);
        else
            session.SetString(key, value);
    }

    public void Clear() =>
        session.Clear();

    public string GetString(string key)
    {
        try
        {
            return session?.GetString(key);
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }

    public SSOUser GetUser()
    {
        string userJson = GetString("ssoUser");

        return !string.IsNullOrEmpty(userJson)
            ? serilizationBroker.Deserialize<SSOUser>(userJson)
            : null;
    }

    public void SetUser(SSOUser user)
    {
        if (user != null)
            session?.SetString("ssoUser", System.Text.Json.JsonSerializer.Serialize(user));
        else
            session?.Remove("ssoUser");
    }

    public void RemoveKey(string key) => 
        session.Remove(key);
}


