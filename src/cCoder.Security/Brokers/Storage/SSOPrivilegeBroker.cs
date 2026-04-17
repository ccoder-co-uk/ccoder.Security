using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage;
internal class SSOPrivilegeBroker(ISecurityDbContextFactory contextFactory) 
    : ISSOPrivilegeBroker
{
    public IQueryable<SSOPrivilege> GetPrivileges() => 
        contextFactory
            .CreateDbContext()
            .GetPrivileges()
            .AsQueryable();
}

