// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;

namespace cCoder.Security.Brokers.Authentication;

internal interface IAuthenticationContextBroker : IUtilityBroker
{
    string GetSSOUserId();
}