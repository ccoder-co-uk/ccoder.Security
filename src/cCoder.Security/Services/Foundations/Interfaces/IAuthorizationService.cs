// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface IAuthorizationService
{
    void EnsureUserCanReadApiMetadata();

    AuthorizationContext GetAuthorizationContext();

    void EnsureUserHasPrivilege(string privilege, string tenantId = null);

    void EnsureUserIsPortalAdminWithPrivilege(string privilege);
}