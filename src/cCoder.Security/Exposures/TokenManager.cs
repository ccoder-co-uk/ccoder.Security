// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;

namespace cCoder.Security.Exposures;

internal class TokenManager(IAuthenticationAggregationService authenticationAggregationService)
    : ITokenManager
{
    public ValueTask<Token> IssueTokenAsync(string userId, TokenUse tokenUse) =>
        authenticationAggregationService.IssueTokenAsync(
            userId: userId,
            tokenUse: tokenUse);
}