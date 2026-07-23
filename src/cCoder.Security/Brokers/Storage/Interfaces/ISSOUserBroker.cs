// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISSOUserBroker
{
    SSOUser SelectCurrentSSOUser();
    IQueryable<SSOUser> SelectAllSSOUsers(bool ignoreFilters = false);

    ValueTask<SSOUser> InsertSSOUserAsync(SSOUser newSSOUser);
    ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser updatedSSOUser);
    ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser);

}