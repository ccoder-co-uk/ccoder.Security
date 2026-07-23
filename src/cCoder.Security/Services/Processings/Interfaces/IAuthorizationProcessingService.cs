// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Models;

namespace cCoder.Security.Services.Processings.Interfaces;

internal interface IAuthorizationProcessingService
{
    AuthorizationContext GetAuthorizationContext();

    void EnsureUserHasPrivilege(string privilege, string tenantId = null);

    void EnsureUserIsPortalAdminWithPrivilege(string privilege);
}