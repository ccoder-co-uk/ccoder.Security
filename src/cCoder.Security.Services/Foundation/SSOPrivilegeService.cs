using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class SSOPrivilegeService : ISSOPrivilegeService
{
    private readonly ISSOPrivilegeBroker privBroker;

    public SSOPrivilegeService(ISSOPrivilegeBroker privBroker)
    {
        this.privBroker = privBroker;
    }

    public IQueryable<SSOPrivilege> GetAllSSOPrivileges()
        => privBroker.GetPrivileges();
}