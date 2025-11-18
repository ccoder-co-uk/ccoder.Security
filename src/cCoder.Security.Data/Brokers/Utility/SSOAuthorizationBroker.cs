using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Utility;

public class SSOAuthorizationBroker(ISecurityDbContextFactory contextFactory) 
    : ISSOAuthorizationBroker
{
    public IEnumerable<SSOPrivilege> GetAllPrivileges()
    {
        var db = contextFactory.CreateDbContext();
        return db.GetPrivileges();
    }

    public SSOUser GetCurrentUser()
    {
        var db = contextFactory.CreateDbContext();
        return db.GetCurrentUser();
    }

    public void UserHasPrivilege(string privilege, string tenantId)
    {
        var db = contextFactory.CreateDbContext();
        db.UserHasPrivilege(privilege, tenantId);
    }

    public void UserIsPortalAdminWithPrivilege(string privilege)
    {
        var db = contextFactory.CreateDbContext();
        db.UserIsPortalAdminWithPrivilege(privilege);
    }
}