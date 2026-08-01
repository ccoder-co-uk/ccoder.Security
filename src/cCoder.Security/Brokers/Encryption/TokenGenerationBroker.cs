// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Dependencies.Encryption;

namespace cCoder.Security.Brokers.Encryption;

internal sealed class TokenGenerationBroker(
    ITokenGenerationDependency tokenGenerationDependency)
    : ITokenGenerationBroker
{
    public string GenerateSelector() =>
        tokenGenerationDependency.GenerateSelector();

    public string GenerateSecret() =>
        tokenGenerationDependency.GenerateSecret();

    public string Combine(string selector, string secret) =>
        $"{selector}.{secret}";

    public string[] Split(string token) =>
        token.Split(
            separator: '.',
            count: 2,
            options: StringSplitOptions.None);
}