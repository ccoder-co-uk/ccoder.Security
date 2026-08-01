// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;

namespace cCoder.Security.Dependencies.Encryption;

internal sealed class TokenGenerationDependency(
    RandomNumberGenerator randomNumberGenerator)
    : ITokenGenerationDependency
{
    private const int SelectorByteCount = 16;
    private const int SecretByteCount = 32;

    public string GenerateSelector() =>
        GenerateToken(byteCount: SelectorByteCount);

    public string GenerateSecret() =>
        GenerateToken(byteCount: SecretByteCount);

    private string GenerateToken(int byteCount)
    {
        byte[] bytes = new byte[byteCount];
        randomNumberGenerator.GetBytes(data: bytes);

        return WebEncoders.Base64UrlEncode(input: bytes);
    }
}