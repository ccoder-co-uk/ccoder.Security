using Microsoft.OData.Edm;
using Security.Objects.Entities;

namespace Security.Api.EDM
{
    public class SecurityModelBuilder : ODataModelBuilder
    {
        public override ODataModel Build() => new()
        {
            Context = "SSO",
            Description = "SSO Endpoints for the Platform.",
            EDMModel = BuildModel()
        };

        IEdmModel BuildModel()
        {
            // common stuff
            AddCommonComplextypes();

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

            return Builder.GetEdmModel();
        }

    }
}