// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Security.Data.EF;

public partial class SecurityDbContext : DbContext
{
    readonly IEnumerable<SSOPrivilege> privileges = [
        Privilege(id:"security_admin", type:"Security", operation:"Admin", description:"Allows users to grant and administer security privileges.", portalAdminsOnly:true),
        Privilege(id:"tenant_create", type:"Tenant", operation:"Create", description:"Allows users to create tenants.", portalAdminsOnly:true),
        Privilege(id:"tenant_read", type:"Tenant", operation:"Read", description:"Allows users to read tenants."),
        Privilege(id:"tenant_update", type:"Tenant", operation:"Update", description:"Allows users to update tenants.", portalAdminsOnly:true),
        Privilege(id:"tenant_delete", type:"Tenant", operation:"Delete", description:"Allows users to delete tenants.", portalAdminsOnly:true),
        Privilege(id:"tenant_admin", type:"Tenant", operation:"Admin", description:"Allows users to administer a tenant.", portalAdminsOnly:true),
        Privilege(id:"tenantsecret_create", type:"TenantSecret", operation:"Create", description:"Allows users to create tenant secrets.", portalAdminsOnly:true),
        Privilege(id:"tenantsecret_read", type:"TenantSecret", operation:"Read", description:"Allows users to read tenant secrets.", portalAdminsOnly:true),
        Privilege(id:"tenantsecret_update", type:"TenantSecret", operation:"Update", description:"Allows users to update tenant secrets.", portalAdminsOnly:true),
        Privilege(id:"tenantsecret_delete", type:"TenantSecret", operation:"Delete", description:"Allows users to delete tenant secrets.", portalAdminsOnly:true),
        Privilege(id:"userrole_create", type:"UserRole", operation:"Create", description:"Allows portal administrators to add users to security roles.", portalAdminsOnly:true),
        Privilege(id:"userrole_read", type:"UserRole", operation:"Read", description:"Allows users to read security role memberships."),
        Privilege(id:"userrole_delete", type:"UserRole", operation:"Delete", description:"Allows portal administrators to remove users from security roles.", portalAdminsOnly:true)
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