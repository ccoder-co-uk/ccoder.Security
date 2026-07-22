using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Security.Data.EF;

public partial class SecurityDbContext : DbContext
{
    readonly IEnumerable<SSOPrivilege> privileges = [
        Privilege("security_admin", "Security", "Admin", "Allows users to grant and administer security privileges.", true),
        Privilege("tenant_create", "Tenant", "Create", "Allows users to create tenants.", true),
        Privilege("tenant_read", "Tenant", "Read", "Allows users to read tenants."),
        Privilege("tenant_update", "Tenant", "Update", "Allows users to update tenants.", true),
        Privilege("tenant_delete", "Tenant", "Delete", "Allows users to delete tenants.", true),
        Privilege("tenant_admin", "Tenant", "Admin", "Allows users to administer a tenant.", true),
        Privilege("tenantsecret_create", "TenantSecret", "Create", "Allows users to create tenant secrets.", true),
        Privilege("tenantsecret_read", "TenantSecret", "Read", "Allows users to read tenant secrets.", true),
        Privilege("tenantsecret_update", "TenantSecret", "Update", "Allows users to update tenant secrets.", true),
        Privilege("tenantsecret_delete", "TenantSecret", "Delete", "Allows users to delete tenant secrets.", true),
        Privilege("userrole_create", "UserRole", "Create", "Allows portal administrators to add users to security roles.", true),
        Privilege("userrole_read", "UserRole", "Read", "Allows users to read security role memberships."),
        Privilege("userrole_delete", "UserRole", "Delete", "Allows portal administrators to remove users from security roles.", true)
    ];

    public IEnumerable<SSOPrivilege> GetPrivileges() => privileges;

    private static SSOPrivilege Privilege(
        string id,
        string type,
        string operation,
        string description,
        bool portalAdminsOnly = false) =>
        new()
        {
            Id = id,
            Type = type,
            Operation = operation,
            Description = description,
            PortalAdminsOnly = portalAdminsOnly
        };
}
