// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal class SSORoleService(
    ISSORoleBroker roleBroker)
        : ISSORoleService
{
    public IQueryable<SSORole> GetAllSSORoles(bool ignoreFilters = false) =>
        roleBroker.SelectAllSSORoles(ignoreFilters: ignoreFilters);

    public async ValueTask<SSORole> AddSSORoleAsync(SSORole newSSORole)
    {
        SSORole storageRole = new()
        {
            Id = newSSORole.Id,
            UsersArePortalAdmins = newSSORole.UsersArePortalAdmins,
            Name = newSSORole.Name,
            Description = newSSORole.Description,
            Privs = newSSORole.Privs,
            TenantId = newSSORole.TenantId
        };

        SSORole result = await roleBroker.InsertSSORoleAsync(newSSORole: storageRole);
        newSSORole.Id = result.Id;
        newSSORole.UsersArePortalAdmins = result.UsersArePortalAdmins;
        newSSORole.Name = result.Name;
        newSSORole.Description = result.Description;
        newSSORole.Privs = result.Privs;
        newSSORole.TenantId = result.TenantId;
        return newSSORole;
    }

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole updatedSSORole)
    {
        SSORole storageRole = new()
        {
            Id = updatedSSORole.Id,
            UsersArePortalAdmins = updatedSSORole.UsersArePortalAdmins,
            Name = updatedSSORole.Name,
            Description = updatedSSORole.Description,
            Privs = updatedSSORole.Privs,
            TenantId = updatedSSORole.TenantId
        };

        SSORole result = await roleBroker.UpdateSSORoleAsync(updatedSSORole: storageRole);
        updatedSSORole.Id = result.Id;
        updatedSSORole.UsersArePortalAdmins = result.UsersArePortalAdmins;
        updatedSSORole.Name = result.Name;
        updatedSSORole.Description = result.Description;
        updatedSSORole.Privs = result.Privs;
        updatedSSORole.TenantId = result.TenantId;
        return updatedSSORole;
    }

    public ValueTask DeleteSSORoleAsync(SSORole deletedSSORole) =>
        roleBroker.DeleteSSORoleAsync(deletedSSORole: deletedSSORole);
}