// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Security.Web.Services.Foundations;

namespace Security.Web.Exposures;

internal sealed class CurrentUserManager(
    ICurrentUserService currentUserService)
        : ICurrentUserManager
{
    public string GetCurrentUserId() =>
        currentUserService.GetCurrentUserId();
}