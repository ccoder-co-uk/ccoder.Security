// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ITenantBroker
{
    ValueTask<Tenant> InsertTenantAsync(Tenant newTenant);
    ValueTask DeleteTenantAsync(Tenant deletedTenant);
    IQueryable<Tenant> SelectAllTenants();
    ValueTask<Tenant> UpdateTenantAsync(Tenant updatedTenant);
}