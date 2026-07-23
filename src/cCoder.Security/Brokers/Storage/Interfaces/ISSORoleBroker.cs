// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISSORoleBroker
{
    ValueTask<SSORole> InsertSSORoleAsync(SSORole newSSORole);
    ValueTask DeleteSSORoleAsync(SSORole deletedSSORole);
    IQueryable<SSORole> SelectAllSSORoles(bool ignoreFilters = false);
    ValueTask<SSORole> UpdateSSORoleAsync(SSORole updatedSSORole);
}