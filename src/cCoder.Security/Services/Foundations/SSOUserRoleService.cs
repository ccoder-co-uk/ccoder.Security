// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal class SSOUserRoleService(ISSOUserRoleBroker userRoleBroker)
    : ISSOUserRoleService
{
    public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole newSSOUserRole)
    {
        SSOUserRole storageUserRole = new()
        {
            RoleId = newSSOUserRole.RoleId,
            UserId = newSSOUserRole.UserId
        };

        SSOUserRole result = await userRoleBroker.InsertSSOUserRoleAsync(userRole: storageUserRole);
        newSSOUserRole.RoleId = result.RoleId;
        newSSOUserRole.UserId = result.UserId;
        return newSSOUserRole;
    }

    public ValueTask DeleteSSOUserRoleAsync(SSOUserRole deletedSSOUserRole) =>
        userRoleBroker.DeleteSSOUserRoleAsync(userRole: deletedSSOUserRole);

    public IQueryable<SSOUserRole> GetAllSSOUserRoles() =>
        userRoleBroker.SelectAllSSOUserRoles();
}