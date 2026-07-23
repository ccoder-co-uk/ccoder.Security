// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface IAuthorizationService
{
    SSOUser GetCurrentUser();

    IEnumerable<SSOPrivilege> GetAllPrivileges();

    void EnsureUserHasPrivilege(string privilege, string tenantId = null);

    void EnsureUserIsPortalAdminWithPrivilege(string privilege);
}