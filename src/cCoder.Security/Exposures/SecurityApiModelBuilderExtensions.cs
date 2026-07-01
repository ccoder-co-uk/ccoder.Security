using cCoder.Security.Objects.Entities;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Security.Exposures;

internal static class SecurityApiModelBuilderExtensions
{
    internal static void ConfigureSecurityApiModel(this ODataConventionModelBuilder builder)
    {
        EntityTypeConfiguration<SSOUser> userType = builder.EntityType<SSOUser>();

        userType.Ignore(u => u.PasswordHash);
        userType.Ignore(u => u.AccessFailedCount);
        userType.Ignore(u => u.Tokens);
        userType.Ignore(u => u.LockoutEnabled);
        userType.Ignore(u => u.LockoutEndDateUtc);

        EntityTypeConfiguration<UserEvent> userEventType = builder.EntityType<UserEvent>();
        userEventType.Ignore(u => u.Session);

        builder.EntitySet<SSOUser>("SSOUser");
        builder.EntitySet<SSORole>("SSORole");
        builder.EntitySet<SSOPrivilege>("SSOPrivilege");
        builder.EntitySet<Tenant>("Tenant");
        builder.EntitySet<TenantAnalysis>("TenantAnalysis");
        builder.EntitySet<UserEvent>("UserEvent");
        builder.EntitySet<SSOUserRole>("SSOUserRole");
        builder.EntityType<SSOUserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
    }

}
