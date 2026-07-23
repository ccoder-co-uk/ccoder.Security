// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal class SessionProcessingService(ISessionService sessionService)
        : ISessionProcessingService
{
    public string GetString(string key) =>
        sessionService.GetString(key: key);

    public SSOUser GetUser() =>
        sessionService.GetUser();

    public void SetString(string key, string value) =>
        sessionService.SetString(key: key, value: value);

    public void SetUser(SSOUser user)
    {
        if (sessionService.GetString(key: "ssoUser") != null)
        { sessionService.RemoveKey(key: "ssoUser"); }

        sessionService.SetUser(user: user);
    }

    public void Remove(string key) =>
        sessionService.RemoveKey(key: key);

    public void Clear() =>
        sessionService.Clear();
}