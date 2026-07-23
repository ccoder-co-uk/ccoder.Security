// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;

internal interface ITenantProcessingService
{
    ValueTask<Tenant> AddTenantAsync(Tenant newTenant);
    ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant);
    ValueTask DeleteTenantAsync(Tenant deletedTenant);
    IQueryable<Tenant> GetAllTenants();
}