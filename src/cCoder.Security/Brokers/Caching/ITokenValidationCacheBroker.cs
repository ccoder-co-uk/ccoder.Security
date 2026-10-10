// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Brokers.Caching;

internal interface ITokenValidationCacheBroker
{
    Token Get(string tokenId);

    void Set(string tokenId, Token token);

    void Remove(string tokenId);

    void Clear();
}