// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface ISSORoleService
{
    IQueryable<SSORole> GetAllSSORoles(bool ignoreFilters = false);

    ValueTask<SSORole> AddSSORoleAsync(SSORole item);
    ValueTask<SSORole> UpdateSSORoleAsync(SSORole item);
    ValueTask DeleteSSORoleAsync(SSORole item);
}