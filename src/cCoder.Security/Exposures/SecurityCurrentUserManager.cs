// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;

namespace cCoder.Security.Exposures;

internal sealed class SecurityCurrentUserManager(
    ICurrentUserAggregationService currentUserAggregationService)
        : ISecurityCurrentUserManager
{
    public SSOUser GetCurrentUser() =>
        currentUserAggregationService.GetCurrentUser();

    public ValueTask<SSOUser> UpdateCurrentSSOUserAsync(SSOUser updatedSSOUser) =>
        currentUserAggregationService.UpdateCurrentSSOUserAsync(
            updatedUser: updatedSSOUser);
}