// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface ISSOUserService
{
    SSOUser Me();
    IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false);

    ValueTask<SSOUser> AddSSOUserAsync(SSOUser item);
    ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser item);
    ValueTask DeleteSSOUserAsync(SSOUser item);
}