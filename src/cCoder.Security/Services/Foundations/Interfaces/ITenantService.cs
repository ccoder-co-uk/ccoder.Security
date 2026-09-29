// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface ITenantService
{
    ValueTask<Tenant> AddTenantAsync(Tenant tenant);
    ValueTask DeleteTenantAsync(Tenant tenant);
    IQueryable<Tenant> GetAllTenants();
    ValueTask<Tenant> UpdateTenantAsync(Tenant tenant);
}