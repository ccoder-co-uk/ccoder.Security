// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using cCoder.CodeAnalysis.Exposures;
using cCoder.Security.Models.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace cCoder.Security.Brokers.Caching;

internal sealed class TokenValidationCacheBroker(
    IMemoryCache memoryCache)
    : ITokenValidationCacheBroker, IUtilityBroker
{
    private CancellationTokenSource cacheCancellation = new();

    public Token Get(string tokenId) =>
        memoryCache.Get<Token>(key: GetKey(tokenId: tokenId));

    public void Set(string tokenId, Token token)
    {
        MemoryCacheEntryOptions options = new()
        {
            AbsoluteExpiration = token.Expires
        };

        options.AddExpirationToken(
            expirationToken: new CancellationChangeToken(
                cancellationToken: cacheCancellation.Token));

        memoryCache.Set(
            key: GetKey(tokenId: tokenId),
            value: token,
            options: options);
    }

    public void Remove(string tokenId) =>
        memoryCache.Remove(key: GetKey(tokenId: tokenId));

    public void Clear()
    {
        CancellationTokenSource previous = Interlocked.Exchange(
            location1: ref cacheCancellation,
            value: new CancellationTokenSource());

        previous.Cancel();
    }

    private static string GetKey(string tokenId) =>
        $"Security:Token:{tokenId}";
}