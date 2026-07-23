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
    public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole item)
    {
        SSOUserRole storageUserRole = new()
        {
            RoleId = item.RoleId,
            UserId = item.UserId
        };

        SSOUserRole result = await userRoleBroker.AddSSOUserRoleAsync(userRole: storageUserRole);
        item.RoleId = result.RoleId;
        item.UserId = result.UserId;
        return item;
    }

    public ValueTask DeleteSSOUserRoleAsync(SSOUserRole item) =>
        userRoleBroker.DeleteSSOUserRoleAsync(userRole: item);

    public IQueryable<SSOUserRole> GetAllSSOUserRoles() =>
        userRoleBroker.GetAllSSOUserRoles();
}