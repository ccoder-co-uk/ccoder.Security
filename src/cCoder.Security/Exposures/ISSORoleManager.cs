// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Exposures;

public interface ISSORoleManager
{
    ValueTask<SSORole> AddSSORoleAsync(SSORole item);
    ValueTask DeleteSSORoleAsync(SSORole item);
    IQueryable<SSORole> GetAllSSORoles();
    ValueTask<SSORole> UpdateSSORoleAsync(SSORole item);
}