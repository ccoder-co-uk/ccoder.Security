// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace cCoder.Security.Dependencies.Sessions;

internal sealed class NullSession : ISession
{
    public bool IsAvailable =>
        false;

    public string Id =>
        string.Empty;

    public IEnumerable<string> Keys =>
        [];

    public void Clear()
    {
    }

    public Task CommitAsync(
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task LoadAsync(
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public void Remove(string key)
    {
    }

    public void Set(string key, byte[] value)
    {
    }

    public bool TryGetValue(
        string key,
        out byte[] value)
    {
        value = null;

        return false;
    }
}