using cCoder.Security.Objects;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace cCoder.Security.Data.EF;

public partial class SecurityDbContext(
    ISSOAuthInfo authInfo, 
    ISecurityModelBuildProvider modelBuildProvider)
        : DbContext
{
    public DbSet<SSOUser> Users { get; set; }
    public DbSet<SSORole> Roles { get; set; }

    public DbSet<SSOUserRole> UserRoles { get; set; }
    public DbSet<Token> Tokens { get; set; }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantAnalysis> TenantAnalysis { get; set; }

    public DbSet<Session> Sessions { get; set; }
    public DbSet<UserEvent> UserEvents { get; set; }

    private SSOUser currentUser;

    private bool UserIsPortalAdmin => GetCurrentUser()
        .Roles
        .Any(r => r.Role.UsersArePortalAdmins);

    public void Migrate() => 
        modelBuildProvider.MigrateDatabase(Database);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuildProvider.Create(modelBuilder);
        ApplyFilters(modelBuilder);
    }

    private void ApplyFilters(ModelBuilder builder)
    {
        builder.Entity<SSOUser>().HasQueryFilter(u => UserIsPortalAdmin || u.Id == authInfo.SSOUserId);
        builder.Entity<SSORole>().HasQueryFilter(r => UserIsPortalAdmin || r.Users.Any());
        builder.Entity<SSOUserRole>().HasQueryFilter(ur => ur.User != null);

        builder.Entity<Token>().HasQueryFilter(t => t.User != null);
        builder.Entity<Session>().HasQueryFilter(s => s.UserEvents.Any());
        builder.Entity<UserEvent>().HasQueryFilter(u => u.CreatedByUser != null);

        builder.Entity<Tenant>().HasQueryFilter(t => t.Roles.Any(r => r.Privs.Contains("tenant_read")));
        builder.Entity<TenantAnalysis>().HasQueryFilter(t => t.Tenant != null);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        modelBuildProvider.Configure(optionsBuilder);

    public SSOUser GetCurrentUser()
    {
        if (currentUser == null || currentUser.Id != authInfo?.SSOUserId)
        {
            string userNameRequested = authInfo?.SSOUserId ?? "Guest";
            if (userNameRequested != "Guest")
                currentUser = Users
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .Include(u => u.Roles)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefault(u => u.Id == userNameRequested);

            currentUser ??= new SSOUser() { Id = "Guest", Roles = Array.Empty<SSOUserRole>() };
        }

        return currentUser;
    }

    protected Guid[] GetCurrentUserRoles() =>
        [.. GetCurrentUser()
            .Roles
            .Select(r => r.RoleId)];

    protected Guid[] GetCurrentUserRolesForTenant(string tenantId) =>
        [.. GetCurrentUser()
            .Roles
            .Where(r => r.Role.TenantId == tenantId)
            .Select(r => r.RoleId)];

    public void UserIsPortalAdminWithPrivilege(string privilege)
    {
        if (!Tenants.IgnoreQueryFilters().Any())
            return;

        var userRoles = GetCurrentUserRoles();

        bool passed = Roles
            .IgnoreQueryFilters()
            .Any(r => userRoles.Contains(r.Id) && (r.Privs.Contains(privilege) && r.UsersArePortalAdmins) || (r.Privs.Contains("security_admin")));

        if (!passed)
            throw new SecurityException($"Privilege '{privilege}' is not granted as current user is not portal admin: '{GetCurrentUser().Id}'");
    }
    public void UserHasPrivilege(string privilege, string tenantId)
    {
        Guid[] userRoles = GetCurrentUserRolesForTenant(tenantId);
        bool passed = Roles.Any(r => userRoles.Contains(r.Id) && (r.Privs.Contains(privilege) || r.Privs.Contains("security_admin")));

        if (!passed)
            throw new SecurityException($"Privilege '{privilege}' is not granted for user: {GetCurrentUser().Id}");
    }

    public IQueryable<UserActivity> GetUserActivity() => 
        UserEvents.IgnoreQueryFilters()
            .Select(ue => new UserActivity
            {
                // tenant details
                TenantId = ue.TenantId,
                TenantName = ue.Tenant.Name,
                TenantDescription = ue.Tenant.Description,
                TenantCreatedBy = ue.Tenant.CreatedBy,
                TenantLastUpdatedBy = ue.Tenant.LastUpdatedBy,
                TenantCreatedOn = ue.Tenant.CreatedOn,
                TenantLastUpdated = ue.Tenant.LastUpdated,

                // user details
                UserId = ue.CreatedBy,
                UserDisplayName = ue.CreatedByUser.DisplayName,
                UserEmail = ue.CreatedByUser.Email,
                UserPhoneNumber = ue.TenantId,

                // event details
                EventId = ue.Id,
                EventName = ue.EventName,
                EventValue = ue.Value,
                EventCreatedOn = ue.CreatedOn,

                // session details
                SessionId = ue.SessionId
            });
}
