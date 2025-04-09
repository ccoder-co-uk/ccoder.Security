using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage;

public class SSOPrivilegeBroker : ISSOPrivilegeBroker
{
    private readonly ISecurityDbContextFactory contextFactory;

    public SSOPrivilegeBroker(ISecurityDbContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public IQueryable<SSOPrivilege> GetPrivileges()
        => contextFactory
            .CreateDbContext()
            .GetPrivileges()
            .AsQueryable();
}