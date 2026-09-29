// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;

namespace cCoder.Security.Exposures;

internal sealed class SecurityCurrentUserManager(
    ISSOUserAggregationService ssoUserAggregationService)
        : ISecurityCurrentUserManager, ICompositionExposure
{
    public SSOUser GetCurrentUser() =>
        ssoUserAggregationService.GetCurrentUser();

    public ValueTask<SSOUser> UpdateCurrentSSOUserAsync(SSOUser updatedSSOUser) =>
        ssoUserAggregationService.UpdateCurrentSSOUserAsync(
            updatedSSOUser: updatedSSOUser);
}