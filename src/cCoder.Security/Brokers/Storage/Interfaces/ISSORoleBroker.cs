// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISSORoleBroker
{
    ValueTask<SSORole> InsertSSORoleAsync(SSORole SSORole);
    ValueTask DeleteSSORoleAsync(SSORole SSORole);
    IQueryable<SSORole> SelectAllSSORoles();
    IQueryable<SSORole> SelectAllSSORolesIgnoringFilters();
    ValueTask<SSORole> UpdateSSORoleAsync(SSORole SSORole);
}