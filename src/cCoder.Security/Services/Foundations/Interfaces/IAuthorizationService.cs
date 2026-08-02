// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using cCoder.Security.Exposures;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface IAuthorizationService
    : IApiMetadataAuthorizationManager
{
    AuthorizationContext GetAuthorizationContext();

    void EnsureUserHasPrivilege(string privilege, string tenantId = null);

    void EnsureUserIsPortalAdminWithPrivilege(string privilege);
}