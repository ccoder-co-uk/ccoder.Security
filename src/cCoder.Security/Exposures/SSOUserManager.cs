// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;

namespace cCoder.Security.Exposures;

internal sealed class SSOUserManager(
    ISSOUserAggregationService ssoUserAggregationService)
        : ISSOUserManager
{
    public IQueryable<SSOUser> GetAllSSOUsers() =>
        ssoUserAggregationService.GetAllSSOUsers();

    public ValueTask<SSOUser> UpdateSSOUserAsync(
        string username,
        SSOUser updatedSSOUser) =>
        ssoUserAggregationService.UpdateSSOUserAsync(
            username: username,
            updatedSSOUser: updatedSSOUser);

    public ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser) =>
        ssoUserAggregationService.DeleteSSOUserAsync(
            deletedSSOUser: deletedSSOUser);
}