// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;

internal interface ISessionProcessingService
{
    void SetString(string key, string value);
    string GetString(string key);
    SSOUser GetUser();
    void SetSSOUser(SSOUser user);
    void Clear();
    void Remove(string key);
}