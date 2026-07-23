// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISSOUserBroker
{
    SSOUser Me();
    IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false);

    ValueTask<SSOUser> AddSSOUserAsync(SSOUser user);
    ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser user);
    ValueTask DeleteSSOUserAsync(SSOUser SSOUser);

}