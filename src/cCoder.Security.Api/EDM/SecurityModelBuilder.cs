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

        var userType = Builder.EntityType<SSOUser>();

        userType.Ignore(u => u.PasswordHash);
        userType.Ignore(u => u.AccessFailedCount);
        userType.Ignore(u => u.Tokens);
        userType.Ignore(u => u.LockoutEnabled);
        userType.Ignore(u => u.LockoutEndDateUtc);

        userType
            .Collection
            .Function("Me")
            .ReturnsFromEntitySet<SSOUser>("SSOUsers");

        userType
            .Collection
            .Action("AcceptInvite");

        var userEventType = Builder.EntityType<UserEvent>();
        userEventType.Ignore(u => u.Session);

        // Security
        AddSet<SSOUser, string>();
        AddSet<SSORole, string>();
        AddSet<SSOPrivilege, string>();
        AddSet<Tenant, string>();
        AddSet<TenantAnalysis, Guid>();
        AddSet<UserEvent, Guid>();

        AddJoinSet<SSOUserRole, object>(ur => new { ur.UserId, ur.RoleId });

        return Builder.GetEdmModel();
    }
}