using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage.Interfaces;

public interface ISSOPrivilegeBroker
{
    IQueryable<SSOPrivilege> GetPrivileges();
}