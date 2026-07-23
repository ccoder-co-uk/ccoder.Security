// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Models;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class AuthorizationProcessingService(
    IAuthorizationService authorizationService)
        : IAuthorizationProcessingService
{
    public AuthorizationContext GetAuthorizationContext() =>
        TryCatch(operation: () =>
        {
            ValidateAuthorizationContextOnGet();

            return authorizationService.GetAuthorizationContext();
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