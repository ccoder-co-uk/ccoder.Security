// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings;

internal sealed partial class TokenProcessingService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateTokenOnAdd(string userId, TokenUse tokenUse) =>
        Validate(inputs: [userId, tokenUse]);

    private static void ValidateTokenOnDelete(string tokenId) =>
        Validate(inputs: tokenId);

    private static void ValidateTokensOnGet(bool ignoreFilters) =>
        Validate(inputs: ignoreFilters);

    private static void ValidateTokenOnGet(string tokenId) =>
        Validate(inputs: tokenId);

    private static void ValidateTokenOnGenerate(string userId, TokenUse tokenUse) =>
        Validate(inputs: [userId, tokenUse]);
}