// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;

internal interface ISSORoleProcessingService
{
    IQueryable<SSORole> GetAllSSORoles(bool ignoreFilters = false);

    ValueTask<SSORole> AddSSORoleAsync(SSORole newSSORole);

    ValueTask<SSORole> UpdateSSORoleAsync(SSORole updatedSSORole);

    ValueTask DeleteSSORoleAsync(SSORole deletedSSORole);
}