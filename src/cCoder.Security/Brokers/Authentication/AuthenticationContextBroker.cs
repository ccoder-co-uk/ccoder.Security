// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects;

namespace cCoder.Security.Brokers.Authentication;

internal sealed class AuthenticationContextBroker(ISSOAuthInfo authInfo)
    : IAuthenticationContextBroker
{
    public string GetSSOUserId() =>
        authInfo?.SSOUserId ?? "Guest";
}