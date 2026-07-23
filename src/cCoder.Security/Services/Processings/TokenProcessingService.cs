// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal class TokenProcessingService(ITokenService tokenService)
    : ITokenProcessingService
{
    public ValueTask<Token> AddTokenForUserIdAsync(string userId, TokenUse tokenUse) =>
        tokenService.AddTokenAsync(userId: userId, tokenUse: tokenUse);

    public async ValueTask DeleteTokenAsync(string tokenId)
    {
        Token token = tokenService.GetAllTokens(ignoreFilters: true)
            .FirstOrDefault(predicate: t => t.Id == tokenId);

        if (token != null)
        { await tokenService.DeleteTokenAsync(item: token); }
    }

    public IQueryable<Token> GetAllTokens(bool ignoreFilters = false) =>
        tokenService.GetAllTokens(ignoreFilters: ignoreFilters);

    public Token GetTokenById(string tokenId)
    {
        Token token = tokenService.GetAllTokens()
            .FirstOrDefault(predicate: t => t.Id == tokenId);

        if (token == null)
        { return null; }

        if (token.Expires < DateTimeOffset.Now)
        { return null; }

        return token;
    }

    public ValueTask<Token> GenerateConfirmationToken(string userId) =>
        tokenService.AddTokenAsync(userId: userId, tokenUse: TokenUse.Confirmation);

    public ValueTask<Token> GenerateInvitationToken(string userId) =>
        tokenService.AddTokenAsync(userId: userId, tokenUse: TokenUse.Invitation, timeout: (7 * 24 * 60));

    public ValueTask<Token> GenerateForgottenPasswordToken(string userId) =>
        tokenService.AddTokenAsync(userId: userId, tokenUse: TokenUse.PasswordReset);

    public Token GetForgottenPasswordToken(string tokenId)
    {
        int reasonCode = (int)TokenUse.PasswordReset;

        Token token = tokenService.GetAllTokens(ignoreFilters: true)
            .FirstOrDefault(predicate: r => r.Reason == reasonCode && r.Id == tokenId);

        if (token.Expires < DateTimeOffset.Now)
        { return null; }

        return token;
    }

    public Token GetConfirmationToken(string tokenId)
    {
        int reasonCode = (int)TokenUse.Confirmation;

        Token token = tokenService.GetAllTokens(ignoreFilters: true)
            .FirstOrDefault(predicate: r => r.Reason == reasonCode && r.Id == tokenId);

        if (token is null || token.Expires < DateTimeOffset.Now)
        { return null; }

        return token;
    }

    public Token GetInvitationToken(string tokenId)
    {
        int reasonCode = (int)TokenUse.Invitation;

        Token token = tokenService.GetAllTokens(ignoreFilters: true)
            .FirstOrDefault(predicate: r => r.Reason == reasonCode && r.Id == tokenId);

        if (token.Expires < DateTimeOffset.Now)
        { return null; }

        return token;
    }
}