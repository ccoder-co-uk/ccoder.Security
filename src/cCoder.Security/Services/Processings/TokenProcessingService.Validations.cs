// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Processings;

internal sealed partial class TokenProcessingService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateTokenForUserIdOnAdd(
        string userId,
        TokenUse tokenUse) =>
        Validate(inputs: [userId, tokenUse]);

    private static void ValidateTokenOnDelete(string tokenId) =>
        Validate(inputs: tokenId);

    private static void ValidateAllTokensOnGet(bool ignoreFilters) =>
        Validate(inputs: ignoreFilters);

    private static void ValidateTokenByIdOnGet(string tokenId) =>
        Validate(inputs: tokenId);

    private static void ValidateTokensForUserOnDelete(
        string userId,
        TokenUse tokenUse) =>
        Validate(inputs: [userId, tokenUse]);

    private static void ValidateForgottenPasswordTokenOnGet(string tokenId) =>
        Validate(inputs: tokenId);

    private static void ValidateConfirmationTokenOnGet(string tokenId) =>
        Validate(inputs: tokenId);

    private static void ValidateInvitationTokenOnGet(string tokenId) =>
        Validate(inputs: tokenId);

    private static void ValidateTokenOnGenerate(string userId, TokenUse tokenUse) =>
        Validate(inputs: [userId, tokenUse]);
}