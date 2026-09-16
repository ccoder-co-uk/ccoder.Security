// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using cCoder.Security.Models;

namespace cCoder.Security.Services.Processings;

internal sealed partial class TokenProcessingService(
    ITokenService tokenService,
    ITokenGenerationBroker tokenGenerationBroker,
    IPasswordHashingBroker passwordHashingBroker,
    SecurityConfiguration securityConfiguration = null)
    : ITokenProcessingService
{
    public ValueTask ExecuteCleanupAsync(
        CancellationToken cancellationToken) =>
        TryCatch(operation: async () =>
        {
            ValidateCleanupOnExecute(
                cancellationToken: cancellationToken);

            if (securityConfiguration?.IsMigrating == true)
            {
                return;
            }

            await tokenService.DeleteExpiredAsync(
                cancellationToken: cancellationToken);

            using PeriodicTimer timer = new(
                period: TimeSpan.FromMinutes(minutes: 1));

            while (!cancellationToken.IsCancellationRequested
                && await timer.WaitForNextTickAsync(
                    cancellationToken: cancellationToken))
            {
                await tokenService.DeleteExpiredAsync(
                    cancellationToken: cancellationToken);
            }
        });

    public ValueTask<Token> AddTokenForUserIdAsync(string userId, TokenUse tokenUse) =>
        TryCatch<Token>(operation: async () =>
        {
            ValidateTokenForUserIdOnAdd(userId: userId, tokenUse: tokenUse);

            return await tokenService.AddTokenAsync(
                userId: userId,
                tokenUse: tokenUse);
        });

    public ValueTask DeleteTokenAsync(string tokenId) =>
        TryCatch(operation: async () =>
        {
            ValidateTokenOnDelete(tokenId: tokenId);

            Token token = GetStoredToken(tokenId: tokenId);

            if (token is not null)
            {
                await tokenService.DeleteTokenAsync(item: token);
            }
        });

    public IQueryable<Token> GetAllTokens(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateAllTokensOnGet(ignoreFilters: ignoreFilters);

            return tokenService.GetAllTokens(ignoreFilters: ignoreFilters);
        });

    public ValueTask DeleteTokensForUserAsync(
        string userId,
        TokenUse tokenUse) =>
        TryCatch(operation: async () =>
        {
            ValidateTokensForUserOnDelete(userId: userId, tokenUse: tokenUse);

            Token[] tokens = tokenService
                .GetAllTokens(ignoreFilters: true)
                .Where(predicate: token =>
                    token.UserName == userId
                    && token.Reason == (int)tokenUse)
                .ToArray();

            foreach (Token token in tokens)
            {
                await tokenService.DeleteTokenAsync(item: token);
            }
        });

    public Token GetTokenById(string tokenId) =>
        TryCatch(operation: () =>
        {
            ValidateTokenByIdOnGet(tokenId: tokenId);

            Token token = GetStoredToken(tokenId: tokenId);

            return token is null || token.Expires < DateTimeOffset.Now
                ? null
                : token;
        });

    public ValueTask<Token> GenerateConfirmationToken(string userId) =>
        TryCatch<Token>(operation: async () =>
        {
            ValidateTokenOnGenerate(
                userId: userId,
                tokenUse: TokenUse.Confirmation);

            return await GenerateTokenAsync(
                userId: userId,
                tokenUse: TokenUse.Confirmation);
        });

    public ValueTask<Token> GenerateInvitationToken(string userId) =>
        TryCatch<Token>(operation: async () =>
        {
            ValidateTokenOnGenerate(
                userId: userId,
                tokenUse: TokenUse.Invitation);

            return await GenerateTokenAsync(
                userId: userId,
                tokenUse: TokenUse.Invitation,
                timeout: 7 * 24 * 60);
        });

    public ValueTask<Token> GenerateForgottenPasswordToken(string userId) =>
        TryCatch<Token>(operation: async () =>
        {
            ValidateTokenOnGenerate(
                userId: userId,
                tokenUse: TokenUse.PasswordReset);

            return await GenerateTokenAsync(
                userId: userId,
                tokenUse: TokenUse.PasswordReset);
        });

    public Token GetForgottenPasswordToken(string tokenId) =>
        TryCatch(operation: () =>
        {
            ValidateForgottenPasswordTokenOnGet(tokenId: tokenId);

            return GetToken(
                tokenId: tokenId,
                tokenUse: TokenUse.PasswordReset);
        });

    public Token GetConfirmationToken(string tokenId) =>
        TryCatch(operation: () =>
        {
            ValidateConfirmationTokenOnGet(tokenId: tokenId);

            return GetToken(
                tokenId: tokenId,
                tokenUse: TokenUse.Confirmation);
        });

    public Token GetInvitationToken(string tokenId) =>
        TryCatch(operation: () =>
        {
            ValidateInvitationTokenOnGet(tokenId: tokenId);

            return GetToken(
                tokenId: tokenId,
                tokenUse: TokenUse.Invitation);
        });

    private ValueTask<Token> GenerateTokenAsync(
        string userId,
        TokenUse tokenUse,
        int? timeout = null) =>
        ReplaceTokenAsync(
            userId: userId,
            tokenUse: tokenUse,
            timeout: timeout);

    private async ValueTask<Token> ReplaceTokenAsync(
        string userId,
        TokenUse tokenUse,
        int? timeout)
    {
        Token[] existingTokens = tokenService
            .GetAllTokens(ignoreFilters: true)
            .Where(predicate: token =>
                token.UserName == userId
                && token.Reason == (int)tokenUse)
            .ToArray();

        foreach (Token existingToken in existingTokens)
        {
            await tokenService.DeleteTokenAsync(item: existingToken);
        }

        return await tokenService.AddTokenAsync(
            userId: userId,
            tokenUse: tokenUse,
            timeout: timeout);
    }

    private Token GetToken(string tokenId, TokenUse tokenUse)
    {
        int reasonCode = (int)tokenUse;
        Token token = GetStoredToken(tokenId: tokenId);

        return token is null
            || token.Reason != reasonCode
            || token.Expires < DateTimeOffset.Now
            ? null
            : token;
    }

    private Token GetStoredToken(string tokenId)
    {
        string[] tokenParts = tokenGenerationBroker.Split(token: tokenId);
        bool isModernToken = tokenParts.Length == 2;
        string selector = isModernToken ? tokenParts[0] : tokenId;

        Token storedToken = tokenService
            .GetAllTokens(ignoreFilters: true)
            .FirstOrDefault(predicate: token => token.Id == selector);

        if (storedToken is null)
        {
            return null;
        }

        if (!isModernToken)
        {
            return string.IsNullOrEmpty(value: storedToken.SecretHash)
                ? storedToken
                : null;
        }

        bool secretMatches = passwordHashingBroker.VerifyTokenSecret(
            secretHash: storedToken.SecretHash,
            providedSecret: tokenParts[1]);

        return secretMatches ? storedToken : null;
    }
}