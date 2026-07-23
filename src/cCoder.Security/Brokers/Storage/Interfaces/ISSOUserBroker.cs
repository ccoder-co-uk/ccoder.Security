// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISSOUserBroker
{
    SSOUser SelectCurrentSSOUser();
    IQueryable<SSOUser> SelectAllSSOUsers(bool ignoreFilters = false);

    ValueTask<SSOUser> InsertSSOUserAsync(SSOUser user);
    ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser user);
    ValueTask DeleteSSOUserAsync(SSOUser SSOUser);

}