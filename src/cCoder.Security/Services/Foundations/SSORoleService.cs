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

    public async ValueTask<SSORole> AddSSORoleAsync(SSORole item)
    {
        SSORole storageRole = new()
        {
            Id = item.Id,
            UsersArePortalAdmins = item.UsersArePortalAdmins,
            Name = item.Name,
            Description = item.Description,
            Privs = item.Privs,
            TenantId = item.TenantId
        };

        SSORole result = await roleBroker.InsertSSORoleAsync(SSORole: storageRole);
        item.Id = result.Id;
        item.UsersArePortalAdmins = result.UsersArePortalAdmins;
        item.Name = result.Name;
        item.Description = result.Description;
        item.Privs = result.Privs;
        item.TenantId = result.TenantId;
        return item;
    }

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole item)
    {
        SSORole storageRole = new()
        {
            Id = item.Id,
            UsersArePortalAdmins = item.UsersArePortalAdmins,
            Name = item.Name,
            Description = item.Description,
            Privs = item.Privs,
            TenantId = item.TenantId
        };

        SSORole result = await roleBroker.UpdateSSORoleAsync(SSORole: storageRole);
        item.Id = result.Id;
        item.UsersArePortalAdmins = result.UsersArePortalAdmins;
        item.Name = result.Name;
        item.Description = result.Description;
        item.Privs = result.Privs;
        item.TenantId = result.TenantId;
        return item;
    }

    public ValueTask DeleteSSORoleAsync(SSORole item) =>
        roleBroker.DeleteSSORoleAsync(SSORole: item);
}