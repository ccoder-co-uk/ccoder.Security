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

        Builder.EntityType<SSOUser>()
            .Ignore(u => u.PasswordHash);

        // Security
        AddSet<SSORole, string>();
        AddSet<SSOPrivilege, string>();
        AddSet<Session, string>();
        AddSet<Tenant, string>();
        AddSet<TenantAnalysis, Guid>();
        AddSet<Token, string>();
        AddSet<UserEvent, Guid>();


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