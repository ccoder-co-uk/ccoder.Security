// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects;

namespace Security.Web.Exposures;

internal sealed class CurrentUserManager(
    ISSOAuthInfo authInfo)
        : ICurrentUserManager
{
    public string GetCurrentUserId() =>
        authInfo.SSOUserId;
}