using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Utility.Interfaces;
internal interface ISSOAuthorizationBroker
{
    SSOUser GetCurrentUser();

    IEnumerable<SSOPrivilege> GetAllPrivileges();

    void UserHasPrivilege(string privilege, string tenantId = null);

    void UserIsPortalAdminWithPrivilege(string privilege);
}

