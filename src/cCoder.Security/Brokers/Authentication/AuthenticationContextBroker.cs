// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using cCoder.Security.Models.Configurations;

namespace cCoder.Security.Brokers.Authentication;

internal sealed class AuthenticationContextBroker(ISSOAuthInfo authInfo)
    : IAuthenticationContextBroker
{
    public string GetSSOUserId() =>
        authInfo?.SSOUserId ?? "Guest";
}