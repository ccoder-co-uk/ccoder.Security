using cCoder.Security.Objects.Entities;
using Microsoft.OData.Edm;

namespace cCoder.Security.Api.EDM;

public class SecurityModelBuilder : ODataModelBuilder
{
    public override ODataModel Build() => new()
    {
        Context = "SSO",
        Description = "SSO Endpoints for the Platform.",
        EDMModel = BuildModel()
    };

    private IEdmModel BuildModel()
    {
        // common stuff
        AddCommonComplexTypes();

        var userSet = Builder.EntityType<SSOUser>();
        userSet.Ignore(u => u.PasswordHash);
        userSet.Ignore(u => u.AccessFailedCount);
        userSet.Ignore(u => u.Tokens);
        userSet.Ignore(u => u.LockoutEnabled);
        userSet.Ignore(u => u.LockoutEndDateUtc);

        // Security
        AddSet<SSORole, string>();
        AddSet<SSOPrivilege, string>();
        AddSet<Tenant, string>();
        AddSet<TenantAnalysis, Guid>();
        AddSet<UserEvent, Guid>();

        var userEventSet = Builder.EntityType<UserEvent>(); 
        userEventSet.Ignore(u => u.Session);

        AddJoinSet<SSOUserRole, object>(ur => new { ur.UserId, ur.RoleId });

        AddSet<SSOUser, string>();

        Builder.EntityType<SSOUser>()
            .Collection
            .Function("Me")
            .ReturnsFromEntitySet<SSOUser>("SSOUsers");

        Builder.EntityType<SSOUser>()
            .Collection
            .Function("AcceptInvite");

        return Builder.GetEdmModel();
    }
}