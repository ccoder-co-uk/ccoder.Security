// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class TokenService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateTokenOnAdd(
        string userId,
        TokenUse tokenUse,
        int? timeout) =>
        Validate(inputs: [userId, tokenUse]);

    private static void ValidateTokenOnDelete(Token deletedToken) =>
        Validate(inputs: deletedToken);

    private static void ValidateExpiredOnDelete(CancellationToken cancellationToken) =>
        Validate(inputs: cancellationToken);

    private static void ValidateAllTokensOnGet(bool ignoreFilters) =>
        Validate(inputs: ignoreFilters);
}