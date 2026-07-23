// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ITokenBroker
{
    ValueTask<Token> InsertTokenAsync(Token token);
    ValueTask DeleteTokenAsync(Token token);
    ValueTask<int> DeleteExpiredAsync(
        DateTimeOffset expiresBefore,
        CancellationToken cancellationToken = default);
    IQueryable<Token> SelectAllTokens(bool ignoreFilters = false);
    ValueTask<Token> UpdateTokenAsync(Token token);
}