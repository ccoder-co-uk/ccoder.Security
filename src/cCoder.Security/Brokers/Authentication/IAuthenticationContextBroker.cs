// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Security.Brokers.Authentication;

internal interface IAuthenticationContextBroker : IUtilityBroker
{
    string GetSSOUserId();
}