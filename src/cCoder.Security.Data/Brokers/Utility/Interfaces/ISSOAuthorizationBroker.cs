using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Utility.Interfaces;

public interface ISSOAuthorizationBroker
{
    SSOUser GetCurrentUser();

    IEnumerable<SSOPrivilege> GetAllPrivileges();

    void UserHasPrivilege(string privilege, string tenantId = null);
}