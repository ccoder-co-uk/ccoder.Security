// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using cCoder.Security.Data.Dependencies.Caching;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace cCoder.Security.Tests.Data;

public sealed partial class LocallyCachedDistributedCacheTests
{
    [Fact]
    public async Task ShouldReadDistributedValueOnceWithinLocalLifetimeAsync()
    {
        // Given

        TrackingDistributedCache distributedCache = new();
        using MemoryCache memoryCache = new(new MemoryCacheOptions());

        LocallyCachedDistributedCache cache = new(
            distributedCache: distributedCache,
            memoryCache: memoryCache);

        await distributedCache.SetAsync(
            key: "session",
            value: [1, 2, 3],
            options: new DistributedCacheEntryOptions());

        // When

        byte[] firstValue = await cache.GetAsync(key: "session");
        byte[] secondValue = await cache.GetAsync(key: "session");

        // Then

        firstValue
            .Should()
            .Equal(expected: [1, 2, 3]);

        secondValue
            .Should()
            .Equal(expected: [1, 2, 3]);

        distributedCache.GetCount
            .Should()
            .Be(expected: 1);
    }

    [Fact]
    public async Task ShouldWriteThroughAndRefreshFromLocalValueAsync()
    {
        // Given

        TrackingDistributedCache distributedCache = new();
        using MemoryCache memoryCache = new(new MemoryCacheOptions());

        LocallyCachedDistributedCache cache = new(
            distributedCache: distributedCache,
            memoryCache: memoryCache);

        // When

        await cache.SetAsync(
            key: "session",
            value: [4, 5, 6],
            options: new DistributedCacheEntryOptions());

        await cache.RefreshAsync(key: "session");
        byte[] value = await cache.GetAsync(key: "session");

        // Then

        value
            .Should()
            .Equal(expected: [4, 5, 6]);

        distributedCache.SetCount
            .Should()
            .Be(expected: 1);

        distributedCache.RefreshCount
            .Should()
            .Be(expected: 0);

        distributedCache.GetCount
            .Should()
            .Be(expected: 0);
    }

    [Fact]
    public async Task ShouldRemoveFromBothCachesAsync()
    {
        // Given

        TrackingDistributedCache distributedCache = new();
        using MemoryCache memoryCache = new(new MemoryCacheOptions());

        LocallyCachedDistributedCache cache = new(
            distributedCache: distributedCache,
            memoryCache: memoryCache);

        await cache.SetAsync(
            key: "session",
            value: [7, 8, 9],
            options: new DistributedCacheEntryOptions());

        // When

        await cache.RemoveAsync(key: "session");
        byte[] value = await cache.GetAsync(key: "session");

        // Then

        value
            .Should()
            .BeNull();

        distributedCache.RemoveCount
            .Should()
            .Be(expected: 1);

        distributedCache.GetCount
            .Should()
            .Be(expected: 1);
    }

    private sealed class TrackingDistributedCache : IDistributedCache
    {
        private readonly Dictionary<string, byte[]> values = [];

        public int GetCount { get; private set; }

        public int RefreshCount { get; private set; }

        public int RemoveCount { get; private set; }

        public int SetCount { get; private set; }

        public byte[] Get(string key)
        {
            GetCount++;
            return values.GetValueOrDefault(key: key);
        }

        public Task<byte[]> GetAsync(
            string key,
            CancellationToken token = default)
        {
            return Task.FromResult(result: Get(key: key));
        }

        public void Refresh(string key)
        {
            RefreshCount++;
        }

        public Task RefreshAsync(
            string key,
            CancellationToken token = default)
        {
            Refresh(key: key);
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            RemoveCount++;
            values.Remove(key: key);
        }

        public Task RemoveAsync(
            string key,
            CancellationToken token = default)
        {
            Remove(key: key);
            return Task.CompletedTask;
        }

        public void Set(
            string key,
            byte[] value,
            DistributedCacheEntryOptions options)
        {
            SetCount++;
            values[key] = value;
        }

        public Task SetAsync(
            string key,
            byte[] value,
            DistributedCacheEntryOptions options,
            CancellationToken token = default)
        {
            Set(key: key, value: value, options: options);
            return Task.CompletedTask;
        }
    }
}