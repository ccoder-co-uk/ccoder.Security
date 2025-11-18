using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class SSOPrivilegeService(ISSOPrivilegeBroker privBroker)
    : ISSOPrivilegeService
{
    public IQueryable<SSOPrivilege> GetAllSSOPrivileges() => 
        privBroker.GetPrivileges();
}