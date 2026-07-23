// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using cCoder.Security.Objects.Entities;
using Microsoft.OData.Edm;

namespace cCoder.Security.Dependencies.EDM;

public class SecurityModelBuilder : ODataModelBuilder
{
    public override ODataModel Build() =>
        new()
        {
            Context = "SSO",
            Description = "SSO Endpoints for the Platform.",
            EDMModel = BuildModel()
        };

    private IEdmModel BuildModel()
    {
        var userType = Builder.EntityType<SSOUser>();

        userType.Ignore(propertyExpression: u => u.PasswordHash);
        userType.Ignore(propertyExpression: u => u.AccessFailedCount);
        userType.Ignore(propertyExpression: u => u.Tokens);
        userType.Ignore(propertyExpression: u => u.LockoutEnabled);
        userType.Ignore(propertyExpression: u => u.LockoutEndDateUtc);

        var userEventType = Builder.EntityType<UserEvent>();
        userEventType.Ignore(propertyExpression: u => u.Session);

        AddSet<SSOUser, string>();
        AddSet<SSORole, string>();
        AddSet<SSOPrivilege, string>();
        AddSet<Tenant, string>();
        AddSet<TenantAnalysis, Guid>();
        AddSet<UserEvent, Guid>();

        AddJoinSet<SSOUserRole, object>(key: ur => new { ur.UserId, ur.RoleId });

        return Builder.GetEdmModel();
    }
}