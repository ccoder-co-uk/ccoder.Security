using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Security.Data.EF;

public partial class SecurityDbContext : DbContext
{
    public IEnumerable<SSOPrivilege> GetPrivileges() =>
    [
        new() { Id = "tenant_create", Description = $"Allows users to Create Tenants." },
        new() { Id = "tenant_Read", Description = $"Allows users to Read a Tenants." },
        new() { Id = "tenant_admin", Description = $"Allows users to Administer a Tenant." }
    ];
}