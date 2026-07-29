// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using cCoder.Security.Models.Configurations;

namespace Security.Web.Exposures;

internal sealed class CurrentUserManager(
    ISSOAuthInfo authInfo)
        : ICurrentUserManager
{
    public string GetCurrentUserId() =>
        authInfo.SSOUserId;
}