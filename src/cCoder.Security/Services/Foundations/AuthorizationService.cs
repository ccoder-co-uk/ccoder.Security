// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class AuthorizationService(
    ISSOAuthorizationBroker authorizationBroker)
        : IAuthorizationService
{
    public SSOUser GetCurrentUser() =>
        TryCatch(operation: () =>
        {
            ValidateCurrentUserOnGet();

            return authorizationBroker.GetCurrentUser();
        });

    public IEnumerable<SSOPrivilege> GetAllPrivileges() =>
        TryCatch(operation: () =>
        {
            ValidatePrivilegesOnGet();

            return authorizationBroker.GetAllPrivileges();
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