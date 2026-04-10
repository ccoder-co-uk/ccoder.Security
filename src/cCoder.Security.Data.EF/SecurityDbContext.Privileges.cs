using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Security.Data.EF;

public partial class SecurityDbContext : DbContext
{
    readonly IEnumerable<SSOPrivilege> privileges = [
        new() { Id = "security_admin", Description = "Allows users to use or add privs they don't already have." },
        new() { Id = "tenant_create", Description = "Allows users to Create Tenants." },
        new() { Id = "tenant_Read", Description = "Allows users to Read a Tenants." },
        new() { Id = "tenant_admin", Description = "Allows users to Administer a Tenant." },
        new() { Id = "tenantsecret_create", Description = "Allows users to Create Tenant Secrets." },
        new() { Id = "tenantsecret_read", Description = "Allows users to Read Tenant Secrets." },
        new() { Id = "tenantsecret_update", Description = "Allows users to Update Tenant Secrets." },
        new() { Id = "tenantsecret_delete", Description = "Allows users to Delete Tenant Secrets." }
    ];

    public IEnumerable<SSOPrivilege> GetPrivileges() => privileges;
}