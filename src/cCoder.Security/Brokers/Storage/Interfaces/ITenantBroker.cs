// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ITenantBroker
{
    ValueTask<Tenant> AddTenantAsync(Tenant tenant);
    ValueTask DeleteTenantAsync(Tenant tenant);
    IQueryable<Tenant> GetAllTenants();
    ValueTask<Tenant> UpdateTenantAsync(Tenant tenant);
}