// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class SSOUserRoleService(ISSOUserRoleBroker userRoleBroker)
    : ISSOUserRoleService
{
    public ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole newSSOUserRole) =>
        TryCatch<SSOUserRole>(operation: async () =>
        {
            ValidateSSOUserRoleOnAdd(newSSOUserRole: newSSOUserRole);

            SSOUserRole storageUserRole = new()
            {
                RoleId = newSSOUserRole.RoleId,
                UserId = newSSOUserRole.UserId
            };

            SSOUserRole result = await userRoleBroker.InsertSSOUserRoleAsync(
                userRole: storageUserRole);

            newSSOUserRole.RoleId = result.RoleId;
            newSSOUserRole.UserId = result.UserId;

            return newSSOUserRole;
        });

    public ValueTask DeleteSSOUserRoleAsync(SSOUserRole deletedSSOUserRole) =>
        TryCatch(operation: async () =>
        {
            ValidateSSOUserRoleOnDelete(deletedSSOUserRole: deletedSSOUserRole);

            await userRoleBroker.DeleteSSOUserRoleAsync(userRole: deletedSSOUserRole);
        });

    public IQueryable<SSOUserRole> GetAllSSOUserRoles() =>
        TryCatch(operation: () => userRoleBroker.SelectAllSSOUserRoles());
}