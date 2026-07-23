// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;

namespace cCoder.Security.Exposures;

internal class TokenManager(IAuthenticationOrchestrationService authenticationOrchestrationService)
    : ITokenManager
{
    public ValueTask<Token> IssueTokenAsync(string userId, TokenUse tokenUse) =>
        authenticationOrchestrationService.IssueTokenAsync(userId: userId, tokenUse: tokenUse);
}