// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;

public interface ITenantCoordinationService
{
    IQueryable<Tenant> GetAllTenants();

    ValueTask<Tenant> AddTenantAsync(Tenant newTenant);

    ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant);

    ValueTask DeleteTenantAsync(Tenant deletedTenant);
}