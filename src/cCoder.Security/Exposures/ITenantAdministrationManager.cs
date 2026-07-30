// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Exposures;

public interface ITenantAdministrationManager
{
    ValueTask<Tenant> AddTenantAsync(Tenant item);

    ValueTask DeleteTenantAsync(Tenant item);

    IQueryable<Tenant> GetAllTenants();

    ValueTask<Tenant> UpdateTenantAsync(Tenant item);
}