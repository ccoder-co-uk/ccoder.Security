// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class AuthorizationProcessingService(
    IAuthorizationService authorizationService)
        : IAuthorizationProcessingService
{
    public SSOUser GetCurrentUser() =>
        TryCatch(operation: () =>
        {
            ValidateCurrentUserOnGet();

            return authorizationService.GetCurrentUser();
        });

    public IEnumerable<SSOPrivilege> GetAllPrivileges() =>
        TryCatch(operation: () =>
        {
            ValidatePrivilegesOnGet();

            return authorizationService.GetAllPrivileges();
        });

    public void EnsureUserHasPrivilege(string privilege, string tenantId = null) =>
        TryCatch(operation: () =>
        {
            ValidatePrivilegeOnEnsure(privilege: privilege, tenantId: tenantId);

            authorizationService.EnsureUserHasPrivilege(
                privilege: privilege,
                tenantId: tenantId);
        });

    public void EnsureUserIsPortalAdminWithPrivilege(string privilege) =>
        TryCatch(operation: () =>
        {
            ValidatePortalPrivilegeOnEnsure(privilege: privilege);

            authorizationService.EnsureUserIsPortalAdminWithPrivilege(
                privilege: privilege);
        });
}