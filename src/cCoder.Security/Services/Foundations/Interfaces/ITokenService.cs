// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface ITokenService
{
    ValueTask<Token> AddTokenAsync(string userId, TokenUse tokenUse, int? newNullable = null);

    ValueTask DeleteTokenAsync(Token deletedToken);

    ValueTask<int> DeleteExpiredAsync(CancellationToken deletedCancellationToken = default);

    IQueryable<Token> GetAllTokens(bool ignoreFilters = false);
}