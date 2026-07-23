// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Serialization;
using cCoder.Security.Brokers.Sessions;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class SessionService(
    IWebSessionBroker sessionBroker,
    ISerializationBroker serializationBroker)
    : ISessionService
{
    public void SetString(string key, string value) =>
        TryCatch(operation: () =>
        {
            ValidateSessionOnSet(key: key, value: value);

            if (value is null)
            {
                sessionBroker.Remove(key: key);
            }
            else
            {
                sessionBroker.SetString(key: key, value: value);
            }
        });

    public void Clear() =>
        TryCatch(operation: () => sessionBroker.Clear());

    public string GetString(string key) =>
        TryCatch(operation: () =>
        {
            ValidateStringOnGet(key: key);

            return sessionBroker.GetString(key: key);
        });

    public SSOUser GetUser() =>
        TryCatch(operation: () =>
        {
            ValidateUserOnGet();
            string userJson = sessionBroker.GetString(key: "ssoUser");

            return !string.IsNullOrEmpty(value: userJson)
                ? serializationBroker.Deserialize<SSOUser>(input: userJson)
                : null;
        });

    public void SetSSOUser(SSOUser user) =>
        TryCatch(operation: () =>
        {
            ValidateSSOUserOnSet(user: user);

            if (user != null)
            {
                string serializedUser = System.Text.Json.JsonSerializer.Serialize(value: user);
                sessionBroker.SetString(key: "ssoUser", value: serializedUser);
            }
            else
            {
                sessionBroker.Remove(key: "ssoUser");
            }
        });

    public void RemoveKey(string key) =>
        TryCatch(operation: () =>
        {
            ValidateSessionOnRemove(key: key);

            sessionBroker.Remove(key: key);
        });
}