// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Models;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class AuthorizationService(
    ISSOAuthorizationBroker authorizationBroker)
        : IAuthorizationService
{
    public AuthorizationContext GetAuthorizationContext() =>
        TryCatch(operation: () =>
        {
            ValidateAuthorizationContextOnGet();

            return new AuthorizationContext
            {
                CurrentUser = authorizationBroker.GetCurrentUser(),
                Privileges = authorizationBroker.GetAllPrivileges()
            };
        });

    public void EnsureUserHasPrivilege(string privilege, string tenantId = null) =>
        TryCatch(operation: () =>
        {
            ValidatePrivilegeOnEnsure(privilege: privilege, tenantId: tenantId);

            authorizationBroker.UserHasPrivilege(
                privilege: privilege,
                tenantId: tenantId);
        });

    public void EnsureUserIsPortalAdminWithPrivilege(string privilege) =>
        TryCatch(operation: () =>
        {
            ValidatePortalPrivilegeOnEnsure(privilege: privilege);

            authorizationBroker.UserIsPortalAdminWithPrivilege(
                privilege: privilege);
        });
}