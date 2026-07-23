// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Brokers.Authentication;

internal interface IAuthenticationContextBroker
{
    string GetSSOUserId();
}