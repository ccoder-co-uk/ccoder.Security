// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface ITenantService
{
    ValueTask<Tenant> AddTenantAsync(Tenant newTenant);
    ValueTask DeleteTenantAsync(Tenant deletedTenant);
    IQueryable<Tenant> GetAllTenants();
    ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant);
}