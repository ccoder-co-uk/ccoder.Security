// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface ISessionService
{
    void SetString(string key, string value);
    string GetString(string key);
    SSOUser GetUser();
    void SetSSOUser(SSOUser user);
    void RemoveKey(string key);
    void Clear();
}