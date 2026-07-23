// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface ISSOUserService
{
    SSOUser Me();
    IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false);

    ValueTask<SSOUser> AddSSOUserAsync(SSOUser newSSOUser);
    ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser updatedSSOUser);
    ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser);
}