// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;

namespace cCoder.Security.Exposures;

internal class TokenManager(IAuthenticationOrchestrationService authenticationOrchestrationService)
    : ITokenManager
{
    public async ValueTask<Token> IssueTokenAsync(string userId, TokenUse tokenUse) =>
        await authenticationOrchestrationService.IssueTokenAsync(userId: userId, tokenUse: tokenUse);
}