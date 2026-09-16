// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security.Cryptography;

namespace cCoder.Security.Dependencies.Encryption;

internal sealed class TokenGenerationDependency(
    RandomNumberGenerator randomNumberGenerator)
    : RandomNumberGenerator
{
    public override void GetBytes(byte[] data) =>
        randomNumberGenerator.GetBytes(data: data);

    public override void GetBytes(byte[] data, int offset, int count) =>
        randomNumberGenerator.GetBytes(
            data: data,
            offset: offset,
            count: count);

    public override void GetNonZeroBytes(byte[] data) =>
        randomNumberGenerator.GetNonZeroBytes(data: data);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            randomNumberGenerator.Dispose();
        }

        base.Dispose(disposing: disposing);
    }
}