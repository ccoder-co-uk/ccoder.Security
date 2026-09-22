// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using cCoder.Security.Models.Configurations;

namespace Security.Web.Services.Foundations;

internal sealed partial class CurrentUserService(
    ISSOAuthInfo authInfo)
        : ICurrentUserService
{
    public string GetCurrentUserId() =>
        TryCatch(operation: () =>
            authInfo.SSOUserId);
}