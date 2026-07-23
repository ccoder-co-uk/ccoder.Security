// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Aggregations.Interfaces;

public interface ITenantAggregationService
{
    ValueTask<Tenant> AddTenantAsync(Tenant item);

    ValueTask DeleteTenantAsync(Tenant item);

    IQueryable<Tenant> GetAllTenants();

    ValueTask<Tenant> UpdateTenantAsync(Tenant item);
}