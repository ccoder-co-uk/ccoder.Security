// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface ISSORoleService
{
    IQueryable<SSORole> GetAllSSORoles(bool ignoreFilters = false);

    ValueTask<SSORole> AddSSORoleAsync(SSORole item);
    ValueTask<SSORole> UpdateSSORoleAsync(SSORole item);
    ValueTask DeleteSSORoleAsync(SSORole item);
}