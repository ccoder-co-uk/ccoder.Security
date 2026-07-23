// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ITokenBroker
{
    ValueTask<Token> InsertTokenAsync(Token newToken);
    ValueTask DeleteTokenAsync(Token deletedToken);
    ValueTask<int> DeleteExpiredAsync(
        DateTimeOffset deletedDateTimeOffset,
        CancellationToken deletedCancellationToken = default);
    IQueryable<Token> SelectAllTokens(bool ignoreFilters = false);
    ValueTask<Token> UpdateTokenAsync(Token updatedToken);
}