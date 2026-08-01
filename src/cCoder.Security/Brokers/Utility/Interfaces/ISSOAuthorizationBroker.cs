// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Brokers.Encryption.Interfaces;

internal interface ISSOAuthorizationBroker
{
    SSOUser GetCurrentUser();

    IEnumerable<SSOPrivilege> GetAllPrivileges();

    void UserHasPrivilege(string privilege, string tenantId = null);

    void UserIsPortalAdminWithPrivilege(string privilege);
}