// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Dependencies.Encryption;
using Microsoft.AspNetCore.WebUtilities;

namespace cCoder.Security.Brokers.Encryption;

internal sealed class TokenGenerationBroker(
    TokenGenerationDependency randomNumberGenerator)
    : ITokenGenerationBroker
{
    private const int SelectorByteCount = 16;
    private const int SecretByteCount = 32;

    public string GenerateSelector() =>
        GenerateToken(byteCount: SelectorByteCount);

    public string GenerateSecret() =>
        GenerateToken(byteCount: SecretByteCount);

    public string Combine(string selector, string secret) =>
        $"{selector}.{secret}";

    public string[] Split(string token) =>
        token.Split(
            separator: '.',
            count: 2,
            options: StringSplitOptions.None);

    private string GenerateToken(int byteCount)
    {
        byte[] bytes = new byte[byteCount];
        randomNumberGenerator.GetBytes(data: bytes);

        return WebEncoders.Base64UrlEncode(input: bytes);
    }
}