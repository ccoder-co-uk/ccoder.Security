// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class SessionProcessingService(ISessionService sessionService)
        : ISessionProcessingService
{
    public string GetString(string key) =>
        TryCatch(operation: () =>
        {
            ValidateStringOnGet(key: key);

            return sessionService.GetString(key: key);
        });

    public SSOUser GetUser() =>
        TryCatch(operation: () =>
        {
            return sessionService.GetUser();
        });

    public void SetString(string key, string value) =>
        TryCatch(operation: () =>
        {
            ValidateStringOnSet(key: key, value: value);
            sessionService.SetString(key: key, value: value);
        });

    public void SetSSOUser(SSOUser sSOUser) =>
        TryCatch(operation: () =>
        {
            ValidateSSOUserOnSet(user: sSOUser);

            if (sessionService.GetString(key: "ssoUser") != null)
            {
                sessionService.RemoveKey(key: "ssoUser");
            }

            sessionService.SetSSOUser(user: sSOUser);
        });

    public void Remove(string key) =>
        TryCatch(operation: () =>
        {
            ValidateSessionOnRemove(key: key);
            sessionService.RemoveKey(key: key);
        });

    public void Clear() =>
        TryCatch(operation: () =>
        {
            sessionService.Clear();
        });
}