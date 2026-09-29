// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Aggregations.Interfaces;

public interface ISSOUserAggregationService
{
    SSOUser GetCurrentUser();

    ValueTask<SSOUser> UpdateCurrentSSOUserAsync(SSOUser updatedSSOUser);

    IQueryable<SSOUser> GetAllSSOUsers();

    ValueTask<SSOUser> UpdateSSOUserAsync(
        string username,
        SSOUser updatedSSOUser);

    ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser);
}