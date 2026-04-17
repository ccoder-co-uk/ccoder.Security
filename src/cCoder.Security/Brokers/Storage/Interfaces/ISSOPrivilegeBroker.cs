using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;
internal interface ISSOPrivilegeBroker
{
    IQueryable<SSOPrivilege> GetPrivileges();
}

