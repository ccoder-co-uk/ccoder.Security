// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISSORoleBroker
{
    ValueTask<SSORole> AddSSORoleAsync(SSORole SSORole);
    ValueTask DeleteSSORoleAsync(SSORole SSORole);
    IQueryable<SSORole> GetAllSSORoles(bool ignoreFilters = false);
    ValueTask<SSORole> UpdateSSORoleAsync(SSORole SSORole);
}