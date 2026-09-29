// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;

internal interface ITenantProcessingService
{
    ValueTask<Tenant> AddTenantAsync(Tenant item);
    ValueTask<Tenant> UpdateTenantAsync(Tenant item);
    ValueTask DeleteTenantAsync(Tenant item);
    IQueryable<Tenant> GetAllTenants();
}