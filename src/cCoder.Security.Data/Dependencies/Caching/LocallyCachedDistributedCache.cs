// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace cCoder.Security.Data.Dependencies.Caching;

internal sealed class LocallyCachedDistributedCache(
    IDistributedCache distributedCache,
    IMemoryCache memoryCache)
        : IDistributedCache
{
    private static readonly TimeSpan LocalCacheDuration =
        TimeSpan.FromSeconds(value: 30);

    public byte[] Get(string key)
    {
        if (TryGetLocally(key: key, value: out byte[] value))
        {
            return value;
        }

        value = distributedCache.Get(key: key);
        CacheLocally(key: key, value: value);

        return Clone(value: value);
    }

    public async Task<byte[]> GetAsync(
        string key,
        CancellationToken token = default)
    {
        if (TryGetLocally(key: key, value: out byte[] value))
        {
            return value;
        }

        value = await distributedCache.GetAsync(
            key: key,
            token: token);

        CacheLocally(key: key, value: value);

        return Clone(value: value);
    }

    public void Refresh(string key)
    {
        if (!memoryCache.TryGetValue(key: key, value: out _))
        {
            distributedCache.Refresh(key: key);
        }
    }

    public Task RefreshAsync(
        string key,
        CancellationToken token = default) =>
            memoryCache.TryGetValue(key: key, value: out _)
                ? Task.CompletedTask
                : distributedCache.RefreshAsync(
                    key: key,
                    token: token);

    public void Remove(string key)
    {
        distributedCache.Remove(key: key);
        memoryCache.Remove(key: key);
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken token = default)
    {
        await distributedCache.RemoveAsync(
            key: key,
            token: token);

        memoryCache.Remove(key: key);
    }

    public void Set(
        string key,
        byte[] value,
        DistributedCacheEntryOptions options)
    {
        distributedCache.Set(
            key: key,
            value: value,
            options: options);

        CacheLocally(key: key, value: value);
    }

    public async Task SetAsync(
        string key,
        byte[] value,
        DistributedCacheEntryOptions options,
        CancellationToken token = default)
    {
        await distributedCache.SetAsync(
            key: key,
            value: value,
            options: options,
            token: token);

        CacheLocally(key: key, value: value);
    }

    private bool TryGetLocally(string key, out byte[] value)
    {
        bool found = memoryCache.TryGetValue(
            key: key,
            value: out byte[] cachedValue);

        value = Clone(value: cachedValue);

        return found;
    }

    private void CacheLocally(string key, byte[] value)
    {
        if (value is null)
        {
            return;
        }

        memoryCache.Set(
            key: key,
            value: Clone(value: value),
            absoluteExpirationRelativeToNow: LocalCacheDuration);
    }

    private static byte[] Clone(byte[] value) =>
        value is null
            ? null
            : (byte[])value.Clone();
}