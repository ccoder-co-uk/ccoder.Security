// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISSOUserBroker
{
    SSOUser SelectCurrentSSOUser();
    IQueryable<SSOUser> SelectAllSSOUsers();
    IQueryable<SSOUser> SelectAllSSOUsersIgnoringFilters();

    ValueTask<SSOUser> InsertSSOUserAsync(SSOUser user);
    ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser user);
    ValueTask DeleteSSOUserAsync(SSOUser SSOUser);

}